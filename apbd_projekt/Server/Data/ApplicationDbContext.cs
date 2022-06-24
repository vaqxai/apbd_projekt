using apbd_projekt.Server.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace apbd_projekt.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<CachedSimpleStock> StockStumps { get; set; }
        public DbSet<CachedStockSearch> CachedSearches { get; set; }

        public DbSet<CachedStock> CachedStocks { get; set; }

    }
}