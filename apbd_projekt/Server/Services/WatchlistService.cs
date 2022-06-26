using apbd_projekt.Server.Data;
using apbd_projekt.Server.Models;

namespace apbd_projekt.Server.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly ApplicationDbContext _context;
        public WatchlistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<string>> getWatchlist(string userEmail)
        {
            var watchlist = _context.Watchlists.Where(w => w.UserEmail == userEmail).ToList();
            return watchlist.Select(w => w.Ticker).ToList();
        }

        public async Task<bool> isInWatchlist(string userEmail, string ticker)
        {
            var watchlist = _context.Watchlists.Where(w => w.UserEmail == userEmail && w.Ticker == ticker).ToList();
            return watchlist.Count > 0;
        }

        public async Task addToWatchlist(string userEmail, string ticker)
        {
            if(await isInWatchlist(userEmail, ticker))
            {
                throw new Exception("ticker is already on user's watchlist");
            }
            
            var watchlist = new Watchlist
            {
                UserEmail = userEmail,
                Ticker = ticker
            };
            _context.Watchlists.Add(watchlist);
            await _context.SaveChangesAsync();
        }

        public async Task removeFromWatchlist(string userEmail, string ticker)
        {
            if(!await isInWatchlist(userEmail, ticker))
            {
                return;
            }

            var watchlist = _context.Watchlists.Where(w => w.UserEmail == userEmail && w.Ticker == ticker).ToList();

            foreach (var w in watchlist)
            {
                _context.Watchlists.Remove(w);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> hasWatchlist(string userEmail)
        {
            var watchlist = _context.Watchlists.Where(w => w.UserEmail == userEmail).ToList();
            return watchlist.Count > 0;
        }
        
    }
}
