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

		[HttpGet]
		public async Task<IActionResult> SearchByTickerPart(string search)
		{
			var res = await _service.searchByTickerPart(search);
			
			if (res.Count() == 0)
            {
				return NotFound("No stocks found with this ticker-part");
            }

            return Ok(res);
        }
	}
}
