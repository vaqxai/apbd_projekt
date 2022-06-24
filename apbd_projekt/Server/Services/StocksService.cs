using apbd_projekt.Server.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using apbd_projekt.Server.Data;
using Microsoft.EntityFrameworkCore;
using apbd_projekt.Server.Models.DTOs;

namespace apbd_projekt.Server.Services
{
    public class StocksService : IStocksService
    {
#nullable disable

        private const int MAX_CACHED_AGE = 86400; // 1 day //TODO: Put this in appsettings.json
        private readonly ApplicationDbContext _context;

        //stock searches

        public StocksService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<CachedSimpleStock>> searchByTickerPart(string tickerPart)
        {
            HttpClient Http = new HttpClient();
            // TODO: Put api key somewhere else
            string Request = "https://api.polygon.io/v3/reference/tickers?";
            Request += "&apiKey=Tnp2_HTpZRIDK2Jcrppho5lLwn1tUqI7";
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

        public async Task<bool> isSearchCached(string tickerPart)
        {
            if (await _context.CachedSearches.FirstOrDefaultAsync(cs => cs.SearchTerm == tickerPart) != null) // could also only search for non-expired cached search results
            {
                if ((await getCachedSearch(tickerPart)).GetAge() <= MAX_CACHED_AGE)
                {
                    return true; // there exists a cached search result that is not expired
                }
                else
                {
                    await removeDeceasedCachedResults(tickerPart);
                    return false;  // there exists an expired cached search result
                }
            }
            else
            {
                return false; // there is no cached search result
            }
        }

        public async Task<CachedStockSearch> getCachedSearch(string tickerPart)
        {
            return await _context.CachedSearches.Include(cs => cs.SearchResult).FirstOrDefaultAsync(cs => cs.SearchTerm == tickerPart);
        }

        public async Task saveSearchResultToCache(string searchTerm, ICollection<CachedSimpleStock> searchResult)
        {

            ICollection<CachedSimpleStock> srwcs = new List<CachedSimpleStock>(); // search result containing cached simplestocks

            var cachedSearch = new CachedStockSearch
            {
                SearchTerm = searchTerm, // dont add SearchResult, since we're tracking SimpleStocks separately apparently (and we have a problem with expiry again)
                CreatedOn = DateTime.Now
            };

            await _context.CachedSearches.AddAsync(cachedSearch);

            foreach (CachedSimpleStock ss in searchResult) // this prevents collisions when our new cached search contains simplestocks which are already cached (should they also have their own expiries?)
            {
                var fromCache = await _context.StockStumps.FirstOrDefaultAsync(sc => sc.Ticker == ss.Ticker);
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

        public async Task removeDeceasedCachedResults(string searchTerm)
        {
            // first remove all deceased stocks which used this search term (in ticker or in company name)
            
            var cachedSimpleStocks = from css in _context.StockStumps.AsEnumerable() // prevent query from being translated to sql immediately
                                     where css.Ticker.Contains(searchTerm) || css.Name.Contains(searchTerm) &&
                                     (css.GetAge() > MAX_CACHED_AGE || css.GetAge() < 0) // "< 0" to protect against integer overflow
                                     select css;

            _context.StockStumps.RemoveRange(cachedSimpleStocks);

            // then remove all cached searches where the deceased stocks were used

            var cachedSearches = from cs in _context.CachedSearches
                                 where cs.SearchResult.Any(ccs => cachedSimpleStocks.Contains(ccs))
                                 select cs;

            _context.CachedSearches.RemoveRange(cachedSearches);
            
            // finally, save all changes

            await _context.SaveChangesAsync();
        }

        public async Task removeSearchResultFromCache(string searchTerm)
        {
            var cachedResult = await _context.CachedSearches.FirstOrDefaultAsync(cs => cs.SearchTerm == searchTerm);
            if(cachedResult != null)
            {
                _context.Remove(cachedResult);
                await _context.SaveChangesAsync();
            }
        }

        // full stocks
        public async Task<Stock> getCachedStock(string ticker)
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

        public async Task<bool> isCached(string ticker)
        {
            if (await _context.Stocks.FirstOrDefaultAsync(s => s.Ticker == ticker) != null)
            {
                if ((await getCachedStock(ticker)).GetAge() <= MAX_CACHED_AGE)
                {
                    return true;
                }
                else
                {
                    await removeDeceasedCachedStocks(ticker);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task removeDeceasedCachedStocks(string ticker)
        {
            var stocks = from s in _context.Stocks
                         where s.Ticker == ticker
                         select s; //should always be one but whatever

            foreach (Stock stock in stocks)
            {
                if ((await getCachedStock(stock.Ticker)).GetAge() > MAX_CACHED_AGE || (await getCachedStock(stock.Ticker)).GetAge() < 0) // protect against integer overflow if someone puts an epoch-0 in the cachedOn date...
                {
                    _context.Stocks.Remove(stock);
                }
            }
        }

        public async Task addToCache(Stock stock)
        {
            var cachedStock = new Stock
            {
                Ticker = stock.Ticker,
                UpdatedOn = DateTime.Now
            };

            //is it necessary to: await _context.Stocks.AddAsync(stock); here?
            await _context.Stocks.AddAsync(cachedStock);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Stock> getFull(string ticker)
        {
            HttpClient httpClient = new HttpClient();
            string Request = "https://api.polygon.io/v2/aggs/ticker/" + ticker.ToUpper() + "/range/1/day/2022-06-20/2022-06-24?apiKey=Tnp2_HTpZRIDK2Jcrppho5lLwn1tUqI7"; //TODO: Move api key somewhere safer
            var resp = await httpClient.GetStreamAsync(Request);
            JsonNode data = await JsonSerializer.DeserializeAsync<JsonNode>(resp);

            JsonArray results = (JsonArray)data["results"];

            JsonNode stonk = results[0];

            Stock stock = new Stock
            {
                Ticker = ((double)stonk["c"]).ToString(),
            };

            return stock;
        }
    }
}