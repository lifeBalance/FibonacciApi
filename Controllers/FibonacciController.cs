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
            [FromRoute] int endIndex = 90,
            [FromRoute] bool useCache = true,
            [FromQuery] int timeout = 800,
            [FromQuery] int maxMemory = 1000)
    {
        // Validate input
        if (startIndex < 0 || endIndex < startIndex)
        {
            return BadRequest("Invalid index range.");
        }
        FibonacciResult result = await _fibonacciService.GenerateSubsequence(startIndex, endIndex, timeout);

        // Sort subsequence in ascending order before returning it
        result.Subsequence?.Sort();

        // Print the subsequence length
        System.Console.WriteLine($"Subsequence length: {result.Subsequence?.Count}");

        var responseBody = new
        {
            StartIndex = startIndex,
            EndIndex = endIndex,
            result.TimeoutOccurred,
            result.Subsequence,
            // UseCache = useCache,
            // MaxMemory = maxMemory,
        };

        // Check if no numbers were generated and a timeout occurred
        if (result.Subsequence == null || result.Subsequence.Count == 0 && result.TimeoutOccurred)
        {
            // Return an error response if no items and timeout occurred
            return StatusCode(408, new { Message = "Timeout occurred before generating any Fibonacci numbers." });
        }

        return Ok(responseBody);
    }
}
