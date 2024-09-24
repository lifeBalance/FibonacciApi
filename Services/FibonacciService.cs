
namespace FibonacciApi.Services
{
    public class FibonacciService
    {
        // Fibonacci sequence: 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, ...
        // Method to calculate the nth Fibonacci number using Binet's Formula
        public static int BinetsFormula(int n)
        {
            double phi = (1 + Math.Sqrt(5)) / 2;
            double psi = (1 - Math.Sqrt(5)) / 2;
            return (int)Math.Round((Math.Pow(phi, n) - Math.Pow(psi, n)) / Math.Sqrt(5));
        }

        // Method to return a subsequence of the Fibonacci sequence
        public async Task<List<int>> GenerateSubsequence(int startIndex, int endIndex)
        {
            // Generate the subsequence using the Binet's formula
            var subsequence = new List<int>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Add 500ms delay to simulate a slow operation
                await Task.Delay(500);
                subsequence.Add(BinetsFormula(i));
            }

            return subsequence;
        }
    }
}