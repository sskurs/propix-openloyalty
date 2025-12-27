using System.Collections.Concurrent;
using System.Text.Json;

namespace Worker.Engines
{
    /// <summary>
    /// Stores rules as JSON strings, but returns deserialized rule objects when requested.
    /// </summary>
    public class JsonRuleEngine : IRuleEngine
    {
        private readonly ConcurrentDictionary<string, string> _jsonRules = new();

        /// <summary>
        /// Save or update a rule in JSON form.
        /// ruleData is converted to JSON before storing.
        /// </summary>
        public void AddOrUpdateRule(string ruleId, object ruleData)
        {
            var json = JsonSerializer.Serialize(ruleData);
            _jsonRules[ruleId] = json;
        }

        /// <summary>
        /// Retrieve the raw JSON rule and return a dynamic object.
        /// </summary>
        public object? GetRule(string ruleId)
        {
            if (_jsonRules.TryGetValue(ruleId, out var json))
            {
                return JsonSerializer.Deserialize<object>(json);
            }

            return null;
        }
    }
}
