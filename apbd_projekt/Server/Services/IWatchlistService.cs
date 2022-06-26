namespace apbd_projekt.Server.Services
{
    public interface IWatchlistService
    {
        public ICollection<string> GetWatchlist(string userEmail);

        public bool IsInWatchlist(string userEmail, string ticker);

        public Task AddToWatchlist(string userEmail, string ticker);

        public bool HasWatchlist(string userEmail);

        public Task RemoveFromWatchlist(string userEmail, string ticker);
    }
}
