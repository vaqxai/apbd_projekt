using apbd_projekt.Server.Models;

namespace apbd_projekt.Server.Services
{
    public interface IStocksService
    {
        public Task<ICollection<Stock>> searchByTickerPart(string tickerPart);
    }
}
