using System.ComponentModel.DataAnnotations;

namespace apbd_projekt.Server.Models
{
    public class CachedStockSearch
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
