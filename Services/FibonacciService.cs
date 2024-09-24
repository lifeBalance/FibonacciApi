namespace FibonacciApi.Services
{
    public class FibonacciResult
    {
        public List<ulong>? Subsequence { get; set; }
        public bool TimeoutOccurred { get; set; }
        public bool MemoryLimitReached { get; set; }
    }

    public class FibonacciService
    {
        // Method to calculate the nth Fibonacci number using Binet's Formula
        public static ulong BinetsFormula(int n)
        {
            double phi = (1 + Math.Sqrt(5)) / 2;
            double psi = (1 - Math.Sqrt(5)) / 2;
            ulong fibonacciNumber = (ulong)Math.Round((Math.Pow(phi, n) - Math.Pow(psi, n)) / Math.Sqrt(5));
            if (fibonacciNumber >= ulong.MaxValue)
            {
                throw new OverflowException("The result is too large to store in a ulong.");
            }
            return fibonacciNumber;
        }

        // Method to return a subsequence of the Fibonacci sequence with timeout handling
        public async Task<FibonacciResult> GenerateSubsequence(
            int startIndex,
            int endIndex,
            int timeoutMilliseconds,
            long maxMemoryUsageBytes)
        {
            var subsequence = new List<ulong>();
            var cts = new CancellationTokenSource();

            // Start a task to cancel the operation after the timeout
            var timeoutTask = Task.Delay(timeoutMilliseconds, cts.Token);

            for (int i = startIndex; i <= endIndex; i++)
            {
                int index = i; // Capture the loop variable

                try
                {
                    // Simulate a slow operation with a 500ms delay for each Fibonacci number
                    // If the cancellation token is signaled (e.g., due to a timeout), 
                    // the delay will be canceled, and an OperationCanceledException will be thrown.
                    await Task.Delay(500, cts.Token);

                    if (timeoutTask.IsCompleted)
                    {
                        // Timeout occurred
                        cts.Cancel(); // Cancel the operation
                        return new FibonacciResult
                        {
                            Subsequence = subsequence,  // Return whatever has been calculated
                            TimeoutOccurred = true     // Indicate that a timeout occurred
                        };
                    }

                    // Check if memory usage has exceeded the limit
                    long currentMemoryUsage = GC.GetTotalMemory(false);

                    int currentMemoryUsageMB = (int)(currentMemoryUsage / 1024 / 1024);           // debug
                    System.Console.WriteLine($"Current memory usage: {currentMemoryUsageMB} MB"); // debug
                    
                    if (currentMemoryUsage >= maxMemoryUsageBytes)
                    {
                        cts.Cancel();                   // Cancel the operation due to memory limit
                        return new FibonacciResult
                        {
                            Subsequence = subsequence,  // Return whatever has been calculated
                            TimeoutOccurred = false,
                            MemoryLimitReached = true   // Memory limit was reached
                        };
                    }

                    ulong fibonacciNumber = BinetsFormula(index);

                    lock (subsequence)
                    {
                        subsequence.Add(fibonacciNumber); // Add the computed Fibonacci number
                    }
                }
                // Handle the cancellation exception due to timeout or memory limit
                catch (OperationCanceledException)
                {
                    // Task was canceled, timeout occurred
                    return new FibonacciResult
                    {
                        Subsequence = subsequence,  // Return partial subsequence
                        TimeoutOccurred = true,     // Indicate timeout
                        MemoryLimitReached = false
                    };
                }
                catch (OverflowException)
                {
                    // Handle overflow exception, but continue processing
                    Console.WriteLine($"Overflow occurred while calculating the {index}th Fibonacci number.");
                }
            }

            // If no timeout occurred, return the full subsequence
            return new FibonacciResult
            {
                Subsequence = subsequence,
                TimeoutOccurred = false,
                MemoryLimitReached = false
            };
        }
    }
}
