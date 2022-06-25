using System.ComponentModel.DataAnnotations.Schema;

namespace apbd_projekt.Server.Models
{
    public class StockDay
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // PK
        [ForeignKey("Stock")]
        public string Ticker { get; set; }
        public Stock Stock { get; set; }

        public DateTime Date { get; set; }

        // Attributes
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public int Volume { get; set; }

    }
}
