using apbd_projekt.Server.Models;

namespace apbd_projekt.Server.Services
{
    public interface IStocksService
    {
        // quick/searchbar stock info

        public static ICollection<Shared.SimpleStock> getSearchResultDTO(ICollection<CachedSimpleStock> searchResult)
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

        public Task<ICollection<CachedSimpleStock>> searchByTickerPart(string tickerPart);

        public Task<bool> isSearchCached(string tickerPart);

        public Task<CachedStockSearch> getCachedSearch(string tickerPart);

        public Task saveSearchResultToCache(string searchTerm, ICollection<CachedSimpleStock> searchResult);

        public Task removeSearchResultFromCache(string searchTerm);

        public Task removeDeceasedCachedResults(string searchTerm);

        // full stock info

        public Task<Shared.Stock> getFull(string ticker);

        public Task<bool> isCached(string ticker);

        public Task<Stock> getCachedStock(string ticker);

        public Task addToCache(Shared.Stock stock);

        public Task removeDeceasedCachedStocks(string ticker);

        public Task<ICollection<StockDay>> getDayInfo(string ticker, int n);

    }
}
