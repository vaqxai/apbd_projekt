namespace apbd_projekt.Server.Models.DTOs
{
    public class SimpleStockDTO
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string Ticker { get; set; }
        public string Name { get; set; }
        public string PrimaryExchange { get; set; }
    }
}
