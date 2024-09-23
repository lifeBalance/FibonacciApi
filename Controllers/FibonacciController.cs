using Microsoft.AspNetCore.Mvc;

namespace FibonacciApi.Controllers;

[ApiController]
[Route("api/fibonacci")]
public class FibonacciController : ControllerBase
{

    [Route("subsequence/{startIndex}/{endIndex}/{useCache}")]
    [HttpGet]
    public async Task<IActionResult> GetSubsequence(
            [FromRoute] int startIndex = 0,
            [FromRoute] int endIndex = 5,
            [FromRoute] bool useCache = true,
            [FromQuery] int timeout = 1000,
            [FromQuery] int maxMemory = 1000)
    {
        // Bounce back the query string parameters
        var response = new
        {
            startIndex,
            endIndex,
            useCache,
            timeout,
            maxMemory
        };

        return await Task.FromResult(Ok(response));
    }
}
