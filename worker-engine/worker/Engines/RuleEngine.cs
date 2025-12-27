using System.Collections.Concurrent;

namespace Worker.Engines
{
    /// <summary>
    /// Very lightweight in-memory rule engine for caching rules.
    /// </summary>
    public class RuleEngine : IRuleEngine
    {
        // stores ruleId → ruleObject
        private readonly ConcurrentDictionary<string, object> _rules = new();

        public void AddOrUpdateRule(string ruleId, object ruleData)
        {
            _rules[ruleId] = ruleData;
        }

        public object? GetRule(string ruleId)
        {
            _rules.TryGetValue(ruleId, out var rule);
            return rule;
        }
    }
}
