using System.Threading.Tasks;

namespace Worker.Engines
{
    // Very small placeholder implementation — always returns true.
    public class SimpleSegmentEngine : ISegmentEngine
    {
        public Task<bool> IsInSegmentAsync(string? segmentJson, string userId)
        {
            // Real implementation: parse segmentJson and evaluate against user data.
            return Task.FromResult(true);
        }
    }
}
