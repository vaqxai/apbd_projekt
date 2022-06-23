using System.ComponentModel.DataAnnotations;

namespace apbd_projekt.Server.Models
{
    public class Stock
    {
        [Key, MaxLength(4), MinLength(4)]
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string PrimaryExchange { get; set; }
    }
}
