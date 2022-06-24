using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apbd_projekt.Server.Models
{
    public class CachedSimpleStock //TODO: Perhaps rename to "StockSingleSearchResult?"
    {
        [Key]
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string PrimaryExchange { get; set; }

        public DateTime CachedOn { get; set; }

        public ICollection<CachedStockSearch> AppearsIn { get; set; }

        public int GetAge()
        {
            return (int)(DateTime.Now - CachedOn).TotalSeconds;
        }
    }
}
