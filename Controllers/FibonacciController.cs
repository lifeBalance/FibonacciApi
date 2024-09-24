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
            [FromQuery] int timeoutSeconds = 8,
            [FromQuery] int maxMemoryMB = 100)
    {
        // Validate input
        if (startIndex < 0 || endIndex < startIndex)
        {
            return BadRequest("Invalid index range.");
        }

        // Convert to the proper units that the service expects
        long maxMemoryBytes = maxMemoryMB * 1024 * 1024;
        int timeoutMilliseconds = timeoutSeconds * 1000;

        FibonacciResult result = await _fibonacciService.GenerateSubsequence(
            startIndex,
            endIndex,
            timeoutMilliseconds, 
            maxMemoryBytes);

        // Sort subsequence in ascending order before returning it
        result.Subsequence?.Sort();

        // Print the subsequence length
        Console.WriteLine($"Subsequence length: {result.Subsequence?.Count}"); // debug

        var responseBody = new
        {
            result.Subsequence,
            result.TimeoutOccurred,
            result.MemoryLimitReached,
            // bounce back parameters (to double check)
            StartIndex = startIndex,
            EndIndex = endIndex,
            UseCache = useCache,
            MaxMemory = maxMemoryMB,
        };

        // Check if no numbers were generated and a timeout occurred
        if (result.Subsequence == null || result.Subsequence.Count == 0 && (result.TimeoutOccurred || result.MemoryLimitReached))
        {
            string message = result.TimeoutOccurred ?
                "Timeout occurred before generating any Fibonacci numbers."
                :
                "Memory limit reached before generating any Fibonacci numbers.";
            // Return an error response if no items and timeout occurred
            return StatusCode(408, new { Message = message });
        }

        return Ok(responseBody);
    }
}
