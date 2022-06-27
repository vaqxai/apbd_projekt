using apbd_projekt.Server.Models;
using apbd_projekt.Shared;
using Stock = apbd_projekt.Server.Models.Stock;
using StockDay = apbd_projekt.Server.Models.StockDay;

namespace apbd_projekt.Server.Services
{
    public interface IStocksService
    {
        // quick/searchbar stock info

        public static ICollection<Shared.SimpleStock> GetSearchResultDTO(ICollection<CachedSimpleStock> searchResult)
        {
            var stonks_out = new List<Shared.SimpleStock>();

            foreach (var stock in searchResult)
            {
                stonks_out.Add(new Shared.SimpleStock
                {
                    Name = stock.Name,
                    Ticker = stock.Ticker,
                    PrimaryExchange = stock.PrimaryExchange,
                });
            }

            return stonks_out;
        }

        public Task<ICollection<CachedSimpleStock>> SearchByTickerPart(string tickerPart);

        public Task<bool> IsSearchCached(string tickerPart);

        public Task<CachedStockSearch> GetCachedSearch(string tickerPart);

        public Task SaveSearchResultToCache(string searchTerm, ICollection<CachedSimpleStock> searchResult);

        public Task RemoveSearchResultFromCache(string searchTerm);

        public Task RemoveDeceasedCachedResults(string searchTerm);

        // full stock info

        public Task<Shared.Stock> GetFull(string ticker);

        public Task<bool> IsCached(string ticker);

        public Task<Stock> GetCachedStock(string ticker);

        public Task AddToCache(Shared.Stock stock);

        public Task RemoveDeceasedCachedStocks(string ticker);

        public Task<ICollection<StockDay>> GetDayInfo(string ticker, int n);

        // articles

        public Task<ICollection<Article>> GetArticles(string ticker, int n);

    }
}
