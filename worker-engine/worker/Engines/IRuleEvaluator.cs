using System.Threading.Tasks;

namespace Worker.Engines
{
    public interface IRuleEvaluator
    {
        Task<bool> EvaluateAsync(string ruleName, object input);
    }
}
