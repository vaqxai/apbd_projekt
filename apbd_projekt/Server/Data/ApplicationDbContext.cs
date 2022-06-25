using apbd_projekt.Server.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace apbd_projekt.Server.Data
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        protected override void OnModelCreating (ModelBuilder mb)
        {
            base.OnModelCreating(mb);
            mb.Entity<StockDay>().HasKey(sd => new { sd.Ticker, sd.Date });
        }

        public DbSet<CachedSimpleStock> StockStumps { get; set; }
        public DbSet<CachedStockSearch> CachedSearches { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockDay> StockDays { get; set; }

    }
}