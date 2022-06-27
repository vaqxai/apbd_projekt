using System.Net;
using apbd_projekt.Server.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using apbd_projekt.Server.Data;
using apbd_projekt.Shared;
using Microsoft.EntityFrameworkCore;
using Stock = apbd_projekt.Server.Models.Stock;
using StockDay = apbd_projekt.Server.Models.StockDay;

namespace apbd_projekt.Server.Services
{
    public class StocksService : IStocksService
    {
#nullable disable

        private const int MAX_CACHED_AGE = 86400; // 1 day //TODO: Put this in appsettings.json
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        //stock searches

        public StocksService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<ICollection<CachedSimpleStock>> SearchByTickerPart(string tickerPart)
        {
            HttpClient Http = new();
            // TODO: Put api key somewhere else
            string Request = "https://api.polygon.io/v3/reference/tickers?";
            Request += "&apiKey=" +_config["Stocks:PolygonAPIKey"];
            Request += "&active=true";
            Request += "&sort=ticker&order=asc&limit=10";
            Request += "&market=stocks";
            Request += "&search=" + tickerPart;
            var resp = await Http.GetStreamAsync(Request);
            JsonNode data = await JsonSerializer.DeserializeAsync<JsonNode>(resp);

            JsonArray results = (JsonArray)data["results"];

            var stonks_out = new List<CachedSimpleStock>();

            foreach (JsonNode stonk in results)
            {
                Console.Out.WriteLine(stonk["ticker"]);
                stonks_out.Add(new CachedSimpleStock
                {
                    Ticker = (string)stonk["ticker"],
                    Name = (string)stonk["name"],
                    PrimaryExchange = (string)stonk["primary_exchange"]
                });
            }

            return stonks_out;
        }

        public async Task<bool> IsSearchCached(string tickerPart)
        {
            if (await _context.CachedStockSearches.FirstOrDefaultAsync(cs => cs.SearchTerm == tickerPart) != null) // could also only search for non-expired cached search results
            {
                if ((await GetCachedSearch(tickerPart)).GetAge() <= MAX_CACHED_AGE)
                {
                    return true; // there exists a cached search result that is not expired
                }
                else
                {
                    await RemoveDeceasedCachedResults(tickerPart);
                    return false;  // there exists an expired cached search result
                }
            }
            else
            {
                return false; // there is no cached search result
            }
        }

        public async Task<CachedStockSearch> GetCachedSearch(string tickerPart)
        {
            return await _context.CachedStockSearches.Include(cs => cs.SearchResult).FirstOrDefaultAsync(cs => cs.SearchTerm == tickerPart);
        }

        public async Task SaveSearchResultToCache(string searchTerm, ICollection<CachedSimpleStock> searchResult)
        {

            ICollection<CachedSimpleStock> srwcs = new List<CachedSimpleStock>(); // search result containing cached simplestocks

            var cachedSearch = new CachedStockSearch
            {
                SearchTerm = searchTerm, // dont add SearchResult, since we're tracking SimpleStocks separately apparently (and we have a problem with expiry again)
                CreatedOn = DateTime.Now
            };

            await _context.CachedStockSearches.AddAsync(cachedSearch);

            foreach (CachedSimpleStock ss in searchResult) // this prevents collisions when our new cached search contains simplestocks which are already cached (should they also have their own expiries?)
            {
                var fromCache = await _context.SimpleStocks.FirstOrDefaultAsync(sc => sc.Ticker == ss.Ticker);
                if ( fromCache != null)
                {
                    srwcs.Add(fromCache);
                } else
                {
                    ss.CachedOn = DateTime.Now;

                    if(ss.AppearsIn == null)
                    {
                        ss.AppearsIn = new List<CachedStockSearch>();
                    }

                    ss.AppearsIn.Add(cachedSearch);

                    if (cachedSearch.SearchResult == null)
                    {
                        cachedSearch.SearchResult = new List<CachedSimpleStock>();
                    }

                    cachedSearch.SearchResult.Add(ss);

                    //_context.StockStumps.Add(ss);
                    srwcs.Add(ss);
                }

                await _context.SaveChangesAsync();
            }

        }

        public async Task RemoveDeceasedCachedResults(string searchTerm)
        {
            // first remove all deceased stocks which used this search term (in ticker or in company name)
            
            var cachedSimpleStocks = from css in _context.SimpleStocks.AsEnumerable() // prevent query from being translated to sql immediately
                                     where css.Ticker.Contains(searchTerm) || css.Name.Contains(searchTerm) &&
                                     (css.GetAge() > MAX_CACHED_AGE || css.GetAge() < 0) // "< 0" to protect against integer overflow
                                     select css;

            _context.SimpleStocks.RemoveRange(cachedSimpleStocks);

            // then remove all cached searches where the deceased stocks were used

            var cachedSearches = from cs in _context.CachedStockSearches
                                 where cs.SearchResult.Any(ccs => cachedSimpleStocks.Contains(ccs))
                                 select cs;

            _context.CachedStockSearches.RemoveRange(cachedSearches);
            
            // finally, save all changes

            await _context.SaveChangesAsync();
        }

        public async Task RemoveSearchResultFromCache(string searchTerm)
        {
            var cachedResult = await _context.CachedStockSearches.FirstOrDefaultAsync(cs => cs.SearchTerm == searchTerm);
            if(cachedResult != null)
            {
                _context.Remove(cachedResult);
                await _context.SaveChangesAsync();
            }
        }

        // full stocks
        public async Task<Stock> GetCachedStock(string ticker)
        {
            var stock = await _context.Stocks.Include(cs => cs.StockDays).FirstOrDefaultAsync(s => s.Ticker == ticker);
            if (stock != null)
            {
                return stock;
            }
            else
            {
                throw new Exception("Stock not found in cache"); // if someone forgets to check if its in cache before getting it
            }
        }

        public async Task UpdateCached(string ticker)
        {
            var cachedStock = await _context.Stocks.FirstOrDefaultAsync(s => s.Ticker == ticker);

            if(cachedStock == null)
            {
                throw new Exception("cannot update cached stock which is null, did you forget to check if it is cached?");
            }

            var newStock = await GetFull(ticker);

            cachedStock.CompanyName = newStock.CompanyName;
            cachedStock.Description = newStock.Description;
            cachedStock.Industry = newStock.Industry;
            cachedStock.LogoURL = newStock.LogoURL;
            cachedStock.UpdatedOn = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsCached(string ticker)
        {
            if (await _context.Stocks.FirstOrDefaultAsync(s => s.Ticker == ticker) != null)
            {
                if ((await GetCachedStock(ticker)).GetAge() <= MAX_CACHED_AGE)
                {
                    return true; // cached and not expired
                }
                else
                {
                    await UpdateCached(ticker); // cached but expired, we'll update it straight away
                    return true;
                }
            }
            else
            {
                return false; // not cached
            }
        }

        public async Task RemoveDeceasedCachedStocks(string ticker)
        {
            var stocks = from s in _context.Stocks
                         where s.Ticker == ticker
                         select s; //should always be one but whatever

            foreach (Stock stock in stocks)
            {
                if ((await GetCachedStock(stock.Ticker)).GetAge() > MAX_CACHED_AGE || (await GetCachedStock(stock.Ticker)).GetAge() < 0) // protect against integer overflow if someone puts an epoch-0 in the cachedOn date...
                {
                    _context.Stocks.Remove(stock);
                }
            }
        }

        public async Task AddToCache(Shared.Stock stock)
        {
            var cachedStock = new Stock
            {
                Ticker = stock.Ticker,
                CompanyName = stock.CompanyName,
                Description = stock.Description,
                Industry = stock.Industry,
                Country = stock.Country,
                LogoURL = stock.LogoURL,
                UpdatedOn = DateTime.Now
            };

            //is it necessary to: await _context.Stocks.AddAsync(stock); here?
            await _context.Stocks.AddAsync(cachedStock);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<StockDay>> GetDayInfo(string ticker, int n)
        {

            var stock = await _context.Stocks.Include(s=> s.StockDays).FirstOrDefaultAsync(s => s.Ticker == ticker);

            HttpClient httpClient = new();
            string Request = "https://api.polygon.io/v2/aggs/ticker/" + ticker.ToUpper() + "/range/1/day/";

            DateTime dateFrom = DateTime.Now.AddDays(-n);
            DateTime dateTo = DateTime.Now;

            string dateFromStr = dateFrom.Year + "-" + dateFrom.Month.ToString("D2") + "-" + dateFrom.Day.ToString("D2");
            string dateToStr = dateTo.Year + "-" + dateTo.Month.ToString("D2") + "-" + dateTo.Day.ToString("D2");

            if (stock.StockDays.Count > 0) // move start date to last day in cache to fill the data we're missing
            {
                dateFromStr = stock.StockDays.OrderBy(sd => sd.Date).Last().Date.AddDays(1).ToString("yyyy-MM-dd");

                if ((stock.StockDays.OrderBy(sd => sd.Date).Last().Date.Date - dateTo.Date).TotalDays < 3) // stop doing everything if we're already up to date (huge offset b/c of exchanges opening closing etc. we want to minimize api calls)
                {
                    return stock.StockDays;
                }
            }

            Request += dateFromStr + "/" + dateToStr;
            Request += "?apiKey=" + _config["Stocks:PolygonAPIKey"];

            var resp = await httpClient.GetStreamAsync(Request);
            JsonNode data = await JsonSerializer.DeserializeAsync<JsonNode>(resp);

            JsonArray results = (JsonArray)data["results"];

            if (stock.StockDays == null)
            {
                stock.StockDays = new List<StockDay>();
            }

            for (int i = 0; i < (int)data["resultsCount"]; i++)
            {
                JsonNode stockDay = results[i];

                var day = new StockDay
                {
                    Open = (double)stockDay["o"],
                    High = (double)stockDay["h"],
                    Low = (double)stockDay["l"],
                    Close = (double)stockDay["c"],
                    Volume = (double)stockDay["v"],
                    Date = DateTimeOffset.FromUnixTimeMilliseconds((long)stockDay["t"]).DateTime
                };

                stock.StockDays.Add(day);
            }

            await _context.SaveChangesAsync();

            return stock.StockDays;
        }
        
        public async Task<Shared.Stock> GetFull(string ticker)
        {
            HttpClient httpClient = new();
            string Request = "https://api.polygon.io/v3/reference/tickers/"+ ticker.ToUpper() +"?apiKey=" + _config["Stocks:PolygonAPIKey"];
            var resp = await httpClient.GetStreamAsync(Request);
            JsonNode data = await JsonSerializer.DeserializeAsync<JsonNode>(resp);

            JsonNode res = data["results"];

            var companyName = (string)res["name"];
            var description = (string)res["description"];
            var industry = (string)res["sic_description"];
            var country = (string)res["locale"];

            var missingImageUrl = "https://upload.wikimedia.org/wikipedia/commons/b/b1/Missing-image-232x150.png"; // move this to appconfig

            var logoUrl = (string)(res["branding"] == null ? missingImageUrl : res["branding"]["logo_url"] ?? missingImageUrl);

            var stock = new Shared.Stock
            {
                Ticker = ticker,
                CompanyName = companyName,
                Description = description,
                Industry = industry,
                Country = country,
                LogoURL = logoUrl,
                StockDays = new List<Shared.StockDay>()
            };

            return stock;
        }

        public async Task<ICollection<Article>> GetArticles(string ticker, int n)
        {
            JsonArray results = null;
            var articles = new List<Article>();

            try
            {
                var client = new HttpClient();
                var request = "https://api.polygon.io/v2/reference/news?ticker="
                              + ticker
                              + "&limit=" + n
                              + "&apiKey=" + _config["Stocks:PolygonAPIKey"];

                var data = await JsonSerializer.DeserializeAsync<JsonNode>(await client.GetStreamAsync(request));
                if (data is not null) results = (JsonArray)data["results"];
            }
            catch
            {
                return articles;
            }

            if (results is null)
            {
                return articles;
            }

            foreach (var result in results)
            {
                try
                {
                    var title = (string)result["title"] ?? "Missing Title";
                    var content = (string)result["description"] ?? "";
                    var link = (string)result["article_url"] ?? "";

                    articles.Add(new Article(
                        title,
                        content,
                        link
                    ));
                } catch
                {
                    continue;
                }
            }

            return articles;
        }
    }
}