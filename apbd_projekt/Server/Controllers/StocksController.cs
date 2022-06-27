using System.Text.Json.Nodes;
using apbd_projekt.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apbd_projekt.Server.Controllers
{

	[Route("api/[controller]")]
	[ApiController]
	public class StocksController : ControllerBase
	{
		
		private readonly IStocksService _service;

		public StocksController(IStocksService service)
		{
			_service = service;
		}

		[HttpGet("search")]
		public async Task<IActionResult> SearchByTickerPart(string query)
		{
			try
			{
                if (await _service.IsSearchCached(query))
				{
					var cachedResults = (await _service.GetCachedSearch(query)).SearchResult;

					if (cachedResults != null)
						if (cachedResults.Count > 0)
						{
							return Ok(IStocksService.GetSearchResultDTO(cachedResults));
						}
						else
						{
							await _service.RemoveSearchResultFromCache(query); // we don't want to keep empty search results cached (why were they cached in the first place?)
						}
				}

				var res = await _service.SearchByTickerPart(query);

				if (res.Count == 0)
				{
					return NotFound("No stocks found with this ticker-part");
				}

				await _service.SaveSearchResultToCache(query, res);

				return Ok(IStocksService.GetSearchResultDTO(res));
			} catch (HttpRequestException e)
            {
                return e.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => NotFound("The requested ticker could not be found"),
                    System.Net.HttpStatusCode.TooManyRequests => StatusCode(429),// there is no method for sending a 429
                    _ => Problem(e.Message),
                };
            }
		}       

		[HttpGet("stock")]
		public async Task<IActionResult> GetByTicker(string ticker)
		{
			try
			{
				if (await _service.IsCached(ticker))
				{
					var cachedStock = await _service.GetCachedStock(ticker);
					var stockDays = await _service.GetDayInfo(ticker, 365); // update stock info

                    if (cachedStock == null)
                    {
						throw new Exception("Cached stock is null");
                    }

                    var cachedCompanyName = cachedStock.CompanyName ?? "";
                    var cachedDescription = cachedStock.Description ?? "";
                    var cachedIndustry = cachedStock.Industry ?? "";
                    var cachedCountry = cachedStock.Country ?? "";
                    var cachedLogoURL = cachedStock.LogoURL ?? "";

                    return Ok(new Shared.Stock
                    {
						Ticker = cachedStock.Ticker,
						CompanyName = cachedCompanyName,
						Description = cachedDescription,
						Industry = cachedIndustry,
						Country = cachedCountry,
						LogoURL = cachedLogoURL,
                        StockDays = (from sd in stockDays
									 select new Shared.StockDay
                                    {
                                        Date = sd.Date,
                                        Open = sd.Open,
                                        High = sd.High,
                                        Low = sd.Low,
                                        Close = sd.Close,
                                        Volume = sd.Volume,
                                    }).ToList()
                    });
				}

				var res = await _service.GetFull(ticker);

				if (res == null)
				{
					return NotFound("No stocks found with this ticker");
				}

				await _service.AddToCache(res);
				var days = await _service.GetDayInfo(ticker, 365); // get last year of dayinfo

				if(res.StockDays == null || res.StockDays.Count == 0)
                {
					res.StockDays = (from sd in days
									 select new Shared.StockDay
									 {
										 Date = sd.Date,
										 Open = sd.Open,
										 High = sd.High,
										 Low = sd.Low,
										 Close = sd.Close,
										 Volume = sd.Volume,
									 }).ToList();
                }

				return Ok(res);
			} catch (HttpRequestException e)
            {
                return e.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => NotFound("The requested ticker could not be found"),
                    System.Net.HttpStatusCode.TooManyRequests => StatusCode(429),// too many requests
                    _ => Problem(e.Message),
                };
            }
		}

        [HttpGet("articles")]
        public async Task<IActionResult> GetArticles(string ticker, int n)
        {

            try
            {
                var articles = await _service.GetArticles(ticker, n);
                return Ok(articles);
            }
            catch (HttpRequestException e)
            {
                return e.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => NotFound("The requested ticker could not be found"),
                    System.Net.HttpStatusCode.TooManyRequests => StatusCode(429),// too many requests
                    _ => Problem(e.Message),
                };
            }
        }
	}
}
