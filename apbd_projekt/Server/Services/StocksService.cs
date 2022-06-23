using apbd_projekt.Server.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace apbd_projekt.Server.Services
{
    public class StocksService : IStocksService
    {
        #nullable disable
        public async Task<ICollection<Stock>> searchByTickerPart(string tickerPart)
        {
            HttpClient Http = new HttpClient();
            // TODO: Put api key somewhere else
            string Request = "https://api.polygon.io/v3/reference/tickers?";
            Request += "&apiKey=Tnp2_HTpZRIDK2Jcrppho5lLwn1tUqI7";
            Request += "&active=true";
            Request += "&sort=ticker&order=asc&limit=10";
            Request += "&market=stocks";
            Request += "&search=" + tickerPart;
            var resp = await Http.GetStreamAsync(Request);
            JsonNode data = await JsonSerializer.DeserializeAsync<JsonNode>(resp);

            JsonArray results = (JsonArray)data["results"];

            var stonks_out = new List<Stock>();

            foreach(JsonNode stonk in results)
            {
                Console.Out.WriteLine(stonk["ticker"]);
                stonks_out.Add(new Stock
                {
                    Ticker = (string)stonk["ticker"],
                    Name = (string)stonk["name"],
                    PrimaryExchange = (string)stonk["primary_exchange"]
                });
            }

            return stonks_out;
        }
    }
}
