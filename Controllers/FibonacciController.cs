using FibonacciApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FibonacciApi.Controllers;

[ApiController]
[Route("api/fibonacci")]
public class FibonacciController : ControllerBase
{

    private readonly FibonacciService _fibonacciService;

    public FibonacciController(FibonacciService fibonacciService)
    {
        _fibonacciService = fibonacciService;
    }

    [Route("subsequence/{startIndex}/{endIndex}/{useCache}")]
    [HttpGet]
    public async Task<IActionResult> GetSubsequence(
            [FromRoute] int startIndex = 0,
            [FromRoute] int endIndex = 7,
            [FromRoute] bool useCache = true,
            [FromQuery] int timeout = 1000,
            [FromQuery] int maxMemory = 1000)
    {
        var subsequence = await _fibonacciService.GenerateSubsequence(startIndex, endIndex);

        var responseBody = new
        {
            StartIndex = startIndex,
            EndIndex = endIndex,
            UseCache = useCache,
            Timeout = timeout,
            MaxMemory = maxMemory,
            Subsequence = subsequence
        };

        return Ok(responseBody);
    }
}
