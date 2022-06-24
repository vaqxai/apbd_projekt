using System.ComponentModel.DataAnnotations;

namespace apbd_projekt.Server.Models
{
    public class Stock
    {
        //TODO: Add fields, what fields are needed?
        [Key]
        public int StockId { get; set; }
        public string Ticker { get; set; }
    }
}
