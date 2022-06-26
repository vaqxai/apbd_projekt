using System.ComponentModel.DataAnnotations;

namespace apbd_projekt.Server.Models
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    public class Stock
    {
        [Key]
        public string Ticker { get; set; }

        public string? CompanyName { get; set; }

        public string? Description { get; set; }
        
        public string? Industry { get; set; }

        public string? Country { get; set; }

        public string? LogoURL { get; set; }

        public DateTime UpdatedOn { get; set; }

        public int GetAge()
        {
            return (int)(DateTime.Now - UpdatedOn).TotalSeconds;
        }

        public ICollection<StockDay> StockDays { get; set; }

    }
}
