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
			Console.Out.WriteLine("Request accepted");
			if (await _service.isSearchCached(query))
            {
				var cachedResults = (await _service.getCachedSearch(query)).SearchResult;

				if(cachedResults != null)
					if(cachedResults.Count() > 0)
					{
						return Ok(IStocksService.getSearchResultDTO(cachedResults)); 
					}else
					{
						await _service.removeSearchResultFromCache(query); // we don't want to keep empty search results cached (why were they cached in the first place?)
					}
            }
            
			var res = await _service.searchByTickerPart(query);
			
			if (res.Count() == 0)
            {
				return NotFound("No stocks found with this ticker-part");
            }

			await _service.saveSearchResultToCache(query, res);

            return Ok(IStocksService.getSearchResultDTO(res));
        }

		[HttpGet("stock")]
		public async Task<IActionResult> GetByTicker(string ticker)
		{
			if (await _service.isCached(ticker))
			{
				return Ok((await _service.getCachedStock(ticker)));
			}

			var res = await _service.getFull(ticker);

			if (res == null)
			{
				return NotFound("No stocks found with this ticker");
			}

			await _service.addToCache(res);

			return Ok(res);
		}            
	}
}
