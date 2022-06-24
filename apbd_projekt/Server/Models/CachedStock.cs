using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apbd_projekt.Server.Models
{
    public class CachedStock
    {
        [Key]
        public int CachedStockId { get; set; }

        [ForeignKey("Stock")]
        public int StockId { get; set; }
        public Stock Stock { get; set; }

        public DateTime CreatedOn { get; set; }

        public int GetAge()
        {
            return (int)(DateTime.Now - CreatedOn).TotalSeconds;
        }

    }
}
