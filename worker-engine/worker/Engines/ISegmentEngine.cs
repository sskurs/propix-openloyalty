using System.Threading.Tasks;

namespace Worker.Engines
{
    public interface ISegmentEngine
    {
        Task<bool> IsInSegmentAsync(string? segmentJson, string userId);
    }
}
