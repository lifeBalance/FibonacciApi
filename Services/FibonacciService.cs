
namespace FibonacciApi.Services
{
    public class FibonacciService
    {
        // Method to generate the Fibonacci sequence up to a specified length
        // 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, ....
        private static async Task<List<int>> GenerateFibonacciSequence(int length)
        {
            // Initialize with the first two numbers in the Fibonacci sequence
            var sequence = new List<int> { 0, 1 };

            // Generate the rest of the sequence starting from the third number
            for (int i = 2; i < length; i++)
            {
                // Add 500ms delay to simulate a slow operation
                await Task.Delay(500);
                sequence.Add(sequence[i - 1] + sequence[i - 2]);
            }

            return sequence;
        }

        // Method to return a subsequence of the Fibonacci sequence
        public async Task<List<int>> GenerateSubsequence(int startIndex, int endIndex)
        {
            var sequence = await GenerateFibonacciSequence(endIndex + 1);
            var subsequence = sequence
                .Skip(startIndex)
                .Take(endIndex - startIndex + 1)
                .ToList();
            return subsequence;
        }
    }
}