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
				Console.Out.WriteLine("Request accepted");
				if (await _service.isSearchCached(query))
				{
					var cachedResults = (await _service.getCachedSearch(query)).SearchResult;

					if (cachedResults != null)
						if (cachedResults.Count > 0)
						{
							return Ok(IStocksService.getSearchResultDTO(cachedResults));
						}
						else
						{
							await _service.removeSearchResultFromCache(query); // we don't want to keep empty search results cached (why were they cached in the first place?)
						}
				}

				var res = await _service.searchByTickerPart(query);

				if (res.Count == 0)
				{
					return NotFound("No stocks found with this ticker-part");
				}

				await _service.saveSearchResultToCache(query, res);

				return Ok(IStocksService.getSearchResultDTO(res));
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
				if (await _service.isCached(ticker))
				{
					var cachedStock = await _service.getCachedStock(ticker);
					await _service.getDayInfo(ticker, 365); // update stock info
					return Ok(new Shared.Stock
                    {
						Ticker = cachedStock.Ticker,
						CompanyName = cachedStock.CompanyName,
						Description = cachedStock.Description,
						Industry = cachedStock.Industry,
						Country = cachedStock.Country,
						LogoURL = cachedStock.LogoURL,
                        StockDays = (from sd in cachedStock.StockDays
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

				var res = await _service.getFull(ticker);

				if (res == null)
				{
					return NotFound("No stocks found with this ticker");
				}

				await _service.addToCache(res);
				await _service.getDayInfo(ticker, 365); // get last year of dayinfo

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
	}
}
