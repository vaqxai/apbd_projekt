using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using apbd_projekt.Server.Services;

namespace apbd_projekt.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistsController : ControllerBase
    {

        private readonly IWatchlistService _service;
        public WatchlistsController(IWatchlistService service)
        {
            _service = service;
        }

        [HttpDelete("{userEmail}/{ticker}")]
        public async Task<IActionResult> DeleteFromWatchlist(string userEmail, string ticker)
        {
            try
            {
                if (!await _service.hasWatchlist(userEmail))
                {
                    return NotFound("user has no watchlist");
                }
                else if(!await _service.isInWatchlist(userEmail, ticker))
                {
                    return NotFound("there's no such ticker in user's watchlist");
                }
                else
                {
                    await _service.removeFromWatchlist(userEmail, ticker);
                    return Ok();
                }
            
            }
            catch (HttpRequestException e)
            {
                return e.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => NotFound("The requested ticker could not be found"),
                    _ => Problem(e.Message),
                };
            }
        }

        [HttpGet("{userEmail}")]
        public async Task<IActionResult> GetWatchlist(string userEmail)
        {
            try
            {
                if(!await _service.hasWatchlist(userEmail))
                {
                    return NotFound("No watchlist found");
                }
                
                Console.Out.WriteLine("Request accepted");
                var watchlist = await _service.getWatchlist(userEmail);
                return Ok(watchlist);
            }
            catch (HttpRequestException e)
            {
                return e.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => NotFound("The requested watchlist could not be found"),
                    _ => Problem(e.Message),
                };
            }
        }

        [HttpPost("{userEmail}/{ticker}")]
        public async Task<IActionResult> AddToWatchlist(string userEmail, string ticker)
        {
            try
            {
                if (await _service.isInWatchlist(userEmail, ticker))
                {
                    return BadRequest("Ticker is already in watchlist");
                }
                else
                {
                    await _service.addToWatchlist(userEmail, ticker);
                    return Ok();
                }
            }
            catch (HttpRequestException e)
            {
                return e.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => NotFound("The requested ticker or watchlist could not be found"),
                    _ => Problem(e.Message),
                };
            }
        }
    }
}
