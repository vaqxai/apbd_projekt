namespace apbd_projekt.Server.Models
{
    public class StockDay
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        
        // PK
        public string Ticker { get; set; }
        public DateOnly Date { get; set; }

        // Attributes
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }

    }
}
