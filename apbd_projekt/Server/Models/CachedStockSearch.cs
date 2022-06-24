using System.ComponentModel.DataAnnotations;

namespace apbd_projekt.Server.Models
{
    public class CachedStockSearch
    {
        [Key]
        public int SearchId { get; set; }
        public string SearchTerm { get; set; }
        public ICollection<CachedSimpleStock> SearchResult { get; set; }
        public DateTime CreatedOn { get; set; }

        public int GetAge()
        {
            return (int)(DateTime.Now - CreatedOn).TotalSeconds;
        }
    }
}
