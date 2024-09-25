namespace FibonacciApi.Services
{
    public class FibonacciResult
    {
        public List<ulong>? Subsequence { get; set; }
        public bool TimeoutOccurred { get; set; }
        public bool MemoryLimitReached { get; set; }
    }

    public class MemoryLimitException : Exception
    {
        public MemoryLimitException()
        {
        }
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
        public async Task<FibonacciResult> GenerateSubsequence(int startIndex, int endIndex, long maxMemoryBytes, int timeoutMilliseconds)
        {
            // int timeoutMilliseconds = timeoutSeconds * 1000;
            // long maxMemoryBytes = maxMemoryMB * 1024 * 1024;
            var subsequence = new List<ulong>();
            var cts = new CancellationTokenSource();

            // Start a task to control the timeout
            var timeoutTask = Task.Delay(timeoutMilliseconds, cts.Token);

            try
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    // Run Fibonacci calculation and timeout task in parallel
                    var fibonacciTask = PerformFibonacciCalculation(i, subsequence, maxMemoryBytes, cts);
                    var completedTask = await Task.WhenAny(fibonacciTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        // Timeout occurred
                        cts.Cancel(); // Cancel the operation
                        return new FibonacciResult
                        {
                            Subsequence = subsequence,  // Return whatever has been calculated
                            TimeoutOccurred = true     // Indicate that a timeout occurred
                        };
                    }
                    // Ensure that the Fibonacci task completed successfully
                    await fibonacciTask;
                }
            }
            catch (MemoryLimitException)
            {
                System.Console.WriteLine($"Memory limit exception"); // debug
                return new FibonacciResult
                {
                    Subsequence = subsequence,
                    MemoryLimitReached = true
                };
            }
            catch (OperationCanceledException ex)
            {
                System.Console.WriteLine($"Operation cancelled: {ex.Message}"); // debug
                if (timeoutTask.IsCompleted)
                {
                    System.Console.WriteLine("Operation was cancelled due to timeout."); // debug
                    return new FibonacciResult
                    {
                        Subsequence = subsequence,
                        TimeoutOccurred = true
                    };
                }
                else
                {
                    System.Console.WriteLine("Operation was cancelled due to memory limit."); // debug
                    return new FibonacciResult
                    {
                        Subsequence = subsequence,
                        MemoryLimitReached = true
                    };
                }
            }

            return new FibonacciResult
            {
                Subsequence = subsequence,
                TimeoutOccurred = false,
                MemoryLimitReached = false
            };
        }
        private static async Task PerformFibonacciCalculation(
            int index,
            List<ulong> subsequence,
            long maxMemoryBytes,
            CancellationTokenSource cts)
        {
            // Simulate a slow operation with a 500ms delay for each Fibonacci number
            await Task.Delay(500, cts.Token);

            // Check if memory usage has exceeded the limit
            long currentMemoryUsage = GC.GetTotalMemory(false);

            System.Console.WriteLine($"Memory usage: {currentMemoryUsage:N0} bytes (max {maxMemoryBytes:N0})"); // debug

            if (currentMemoryUsage >= maxMemoryBytes)
            {
                System.Console.WriteLine("Memory limit reached."); // debug
                // Couldn't make it work with the code below (interesting to know why...)
                throw new MemoryLimitException();

                // Request cancellation
                // cts.Cancel();
                // System.Console.WriteLine("Cancellation requested."); // debug
                // cts.Token.ThrowIfCancellationRequested();
            }

            ulong fibonacciNumber = BinetsFormula(index);

            lock (subsequence)
            {
                subsequence.Add(fibonacciNumber); // Add the computed Fibonacci number
            }
        }
    }
}
