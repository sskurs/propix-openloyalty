using RulesEngine.Models;
using RulesEngine;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worker.Engines
{
    public class RulesEngineEvaluator : IRuleEvaluator
    {

        private readonly RulesEngine.RulesEngine _engine;
         
        public RulesEngineEvaluator(IEnumerable<Workflow> workflows)
        {
            Console.WriteLine("RulesEngineEvaluator initialized");
            Console.WriteLine(workflows.Count());
             Console.WriteLine(workflows.ToArray());
            _engine = new RulesEngine.RulesEngine(workflows.ToArray());
        }

public async Task<bool> EvaluateAsync(string ruleName, object input)
{
    try {
         Console.WriteLine("RulesEngineEvaluator EvaluateAsync {ruleName}" + ruleName);  
          var result = await _engine.ExecuteAllRulesAsync(ruleName,new RuleParameter("input", input));

            if (result == null || result.Count == 0)
                return false;

            return result.All(r => r.IsSuccess);
         
         } catch (ArgumentException ex)     
         
                when (ex.Message.Contains("config file is not present", StringComparison.OrdinalIgnoreCase))
                {
                    // Treat “no workflow” as skip instead of crash
                    return false;
                }
}

        public static List<Workflow> BuildWorkflows(string jsonRule)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonRule) || jsonRule == "{}")
                    return new List<Workflow>();

                using (var doc = JsonDocument.Parse(jsonRule))
                {
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        var workflows = JsonSerializer.Deserialize<List<Workflow>>(jsonRule);
                        return workflows ?? new List<Workflow>();
                    }
                    else
                    {
                        var workflow = JsonSerializer.Deserialize<Workflow>(jsonRule);
                        return workflow != null ? new List<Workflow> { workflow } : new List<Workflow>();
                    }
                }
            }
            catch
            {
                return new List<Workflow>();
            }
        }
    }
}
