using FibonacciApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FibonacciApi.Controllers;

[ApiController]
[Route("api/fibonacci")]
public class FibonacciController : ControllerBase
{

    private readonly FibonacciService _fibonacciService;
    private readonly IMemoryCache _memoryCache;

    public FibonacciController(FibonacciService fibonacciService, IMemoryCache memoryCache)
    {
        _fibonacciService = fibonacciService;
        _memoryCache = memoryCache;
    }

    [Route("subsequence/{startIndex}/{endIndex}/{useCache}")]
    [HttpGet]
    public async Task<IActionResult> GetSubsequence(
            [FromRoute] int startIndex = 0,
            [FromRoute] int endIndex = 20,
            [FromRoute] bool useCache = true,
            [FromQuery] int timeoutSeconds = 8,
            [FromQuery] int maxMemoryMB = 100)
    {
        // Validate input
        if (startIndex < 0 || endIndex < startIndex)
        {
            return BadRequest("Invalid index range.");
        }

        string cacheKey = $"Fibonacci_{startIndex}_{endIndex}";

        // Check if the result is already in the cache
        if (useCache && _memoryCache.TryGetValue(cacheKey, out FibonacciResult? cachedResult))
        {
            if (cachedResult != null)
            {
                // Cache hit - return the cached result
                return Ok(new
                {
                    cachedResult.Subsequence,
                    Cached = true,
                });
            }
        }

        FibonacciResult result = await _fibonacciService.GenerateSubsequence(
            startIndex,
            endIndex,
            maxMemoryMB,
            timeoutSeconds);

        // Sort subsequence (in place) in ascending order before returning it
        result.Subsequence?.Sort();

        // Cache only complete subsequences
        if (result.Subsequence != null && result.Subsequence.Count == endIndex - startIndex + 1)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Cache invalidation after 5 minutes of inactivity
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(cacheKey, result, cacheEntryOptions);
        }

        // Print the subsequence length
        // Console.WriteLine($"Subsequence length: {result.Subsequence?.Count}"); // debug

        var responseBody = new Dictionary<string, object>();

        if (result.Subsequence != null && result.Subsequence.Count > 0)
            responseBody["Subsequence"] = result.Subsequence;

        if (result.TimeoutOccurred)
            responseBody["TimeoutOccurred"] = result.TimeoutOccurred;

        if (result.MemoryLimitReached)
            responseBody["MemoryLimitReached"] = result.MemoryLimitReached;

        System.Console.WriteLine("subsequence is", result.Subsequence);
        // Check if no numbers were generated and a timeout occurred
        if ((result.Subsequence == null || result.Subsequence.Count == 0) && (result.TimeoutOccurred || result.MemoryLimitReached))
        {
            string message = result.TimeoutOccurred ?
                "Timeout occurred before generating any Fibonacci numbers."
                :
                "Memory limit reached before generating any Fibonacci numbers.";
            // Return an error response if no items and timeout occurred
            return StatusCode(408, new
            {
                Message = message
            });
        }

        return Ok(responseBody);
    }
}
