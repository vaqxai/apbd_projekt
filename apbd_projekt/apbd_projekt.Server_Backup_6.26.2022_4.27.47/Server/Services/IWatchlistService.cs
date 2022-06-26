namespace apbd_projekt.Server.Services
{
    public interface IWatchlistService
    {
        public Task<ICollection<string>> getWatchlist(string userEmail);

        public Task<bool> isInWatchlist(string userEmail, string ticker);

        public Task addToWatchlist(string userEmail, string ticker);

        public Task<bool> hasWatchlist(string userEmail);

        public Task removeFromWatchlist(string userEmail, string ticker);
    }
}
