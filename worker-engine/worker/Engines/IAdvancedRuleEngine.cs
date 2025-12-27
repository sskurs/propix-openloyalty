using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worker.Engines
{
    // Keeps original minimal IRuleEngine in place.
    public interface IAdvancedRuleEngine : IRuleEngine
    {
        /// <summary>
        /// Evaluate rule by id against provided event/context.
        /// Returns EvaluationResult with actions to execute.
        /// </summary>
        Task<EvaluationResult> EvaluateAsync(string ruleId, IDictionary<string, object?> context);
        IEnumerable<string> GetAllRuleIds();
    }
}
