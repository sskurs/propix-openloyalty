using Worker.Data;
using Microsoft.EntityFrameworkCore;
using Worker.Engines;
using RulesEngine.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Worker.Repositories
{
    public class CampaignRuleRepository
    {
        private readonly WorkerDbContext _db;

        public CampaignRuleRepository(WorkerDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Workflow>> LoadActiveCampaignWorkflowsAsync()
        {
            // Join Campaigns and CampaignConditions to get active rules
            // Load both RuleGroupJson (advanced rules) and ConditionJson (simple rules)
            
            // First, get active campaign IDs as Guid
            var activeCampaigns = await _db.Campaigns
                .Where(c => c.Status == "ACTIVE")
                .Select(c => new { Id = c.Id })
                .ToListAsync();

            Console.WriteLine($"[CampaignRuleRepository] Found {activeCampaigns.Count} ACTIVE campaigns");
            
            // Convert string IDs to Guid and join with conditions
            var campaignGuids = activeCampaigns.Select(c => Guid.Parse(c.Id)).ToList();
            Console.WriteLine($"[CampaignRuleRepository] Converted to {campaignGuids.Count} GUIDs");
            
            // Check how many conditions exist
            var allConditions = await _db.CampaignConditions.CountAsync();
            Console.WriteLine($"[CampaignRuleRepository] Total conditions in database: {allConditions}");
            
            var items = await _db.CampaignConditions
                .Where(cond => campaignGuids.Contains(cond.CampaignId))
                .Select(cond => new { 
                    Id = cond.CampaignId.ToString(), 
                    cond.RuleGroupJson, 
                    cond.ConditionJson 
                })
                .ToListAsync();

            var workflows = new List<Workflow>();

            Console.WriteLine($"[CampaignRuleRepository] Loaded {items.Count} campaign conditions");

            foreach (var item in items)
            {
                Console.WriteLine($"[CampaignRuleRepository] Processing campaign {item.Id}");
                Console.WriteLine($"[CampaignRuleRepository] RuleGroupJson: {(string.IsNullOrWhiteSpace(item.RuleGroupJson) ? "NULL/EMPTY" : "HAS DATA")}");
                Console.WriteLine($"[CampaignRuleRepository] ConditionJson: {(string.IsNullOrWhiteSpace(item.ConditionJson) ? "NULL/EMPTY" : item.ConditionJson)}");
                
                // First, try to load from RuleGroupJson (advanced rules)
                if (!string.IsNullOrWhiteSpace(item.RuleGroupJson) && item.RuleGroupJson != "{}")
                {
                    var wfs = RulesEngineEvaluator.BuildWorkflows(item.RuleGroupJson);
                    if (wfs != null && wfs.Count > 0)
                    {
                        Console.WriteLine($"[CampaignRuleRepository] Loaded {wfs.Count} workflows from RuleGroupJson");
                        workflows.AddRange(wfs);
                        continue; // Skip ConditionJson if RuleGroupJson exists
                    }
                }

                // Fallback: Convert simple ConditionJson to MS RulesEngine workflow
                if (!string.IsNullOrWhiteSpace(item.ConditionJson) && item.ConditionJson != "{}")
                {
                    Console.WriteLine($"[CampaignRuleRepository] Attempting to convert ConditionJson to workflow");
                    var workflow = ConvertSimpleConditionToWorkflow(item.Id, item.ConditionJson);
                    if (workflow != null)
                    {
                        Console.WriteLine($"[CampaignRuleRepository] Successfully created workflow for campaign {item.Id}");
                        workflows.Add(workflow);
                    }
                    else
                    {
                        Console.WriteLine($"[CampaignRuleRepository] Failed to create workflow for campaign {item.Id}");
                    }
                }
            }

            Console.WriteLine($"[CampaignRuleRepository] Total workflows loaded: {workflows.Count}");
            return workflows;
        }

        private Workflow? ConvertSimpleConditionToWorkflow(string campaignId, string conditionJson)
        {
            try
            {
                // Parse the simple condition JSON - it's an array of conditions
                var conditions = System.Text.Json.JsonSerializer.Deserialize<List<SimpleCondition>>(conditionJson);
                if (conditions == null || conditions.Count == 0)
                {
                    Console.WriteLine($"[CampaignRuleRepository] No conditions found or empty array");
                    return null;
                }

                Console.WriteLine($"[CampaignRuleRepository] Parsed {conditions.Count} conditions");

                // Build expressions for each condition
                var expressions = new List<string>();
                foreach (var condition in conditions)
                {
                    var expr = BuildExpression(condition);
                    if (!string.IsNullOrEmpty(expr))
                    {
                        expressions.Add(expr);
                        Console.WriteLine($"[CampaignRuleRepository] Built expression: {expr}");
                    }
                }

                if (expressions.Count == 0)
                {
                    Console.WriteLine($"[CampaignRuleRepository] No valid expressions built");
                    return null;
                }

                // Combine all expressions with AND
                var combinedExpression = expressions.Count == 1 
                    ? expressions[0] 
                    : string.Join(" && ", expressions.Select(e => $"({e})"));

                Console.WriteLine($"[CampaignRuleRepository] Final expression: {combinedExpression}");

                // Convert to MS RulesEngine format
                var rule = new Rule
                {
                    RuleName = $"Rule_{campaignId}",
                    Expression = combinedExpression,
                    RuleExpressionType = RuleExpressionType.LambdaExpression
                };

                return new Workflow
                {
                    WorkflowName = $"campaign_{campaignId}",
                    Rules = new List<Rule> { rule }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CampaignRuleRepository] Exception converting workflow: {ex.Message}");
                return null;
            }
        }

        private string BuildExpression(SimpleCondition condition)
        {
            // Build lambda expression from simple condition
            // Handle different condition types
            
            if (condition.Type == "unknown" || string.IsNullOrWhiteSpace(condition.Type))
            {
                return ""; // Skip unknown conditions
            }

            // Map condition type to input property
            var field = condition.Type switch
            {
                "event.payload.gross_value" => "Amount",
                "transaction.amount" => "Amount",
                "member.Tier" => "Tier",
                "member.PointsBalance" => "PointsBalance",
                "member.TransactionCount" => "TransactionCount",
                "member.TotalSpent" => "TotalSpent",
                _ => null
            };

            if (field == null)
            {
                Console.WriteLine($"[CampaignRuleRepository] Unknown field type: {condition.Type}");
                return "";
            }

            // Parse the value (it's often JSON-encoded)
            var value = condition.Value;
            if (value.StartsWith("{") && value.Contains("\"value\""))
            {
                try
                {
                    var valueObj = System.Text.Json.JsonDocument.Parse(value);
                    if (valueObj.RootElement.TryGetProperty("value", out var valueProp))
                    {
                        value = valueProp.ToString();
                    }
                }
                catch
                {
                    // Use as-is if parsing fails
                }
            }

            // Build expression based on operator
            return condition.Operator switch
            {
                "gte" => $"input.{field} >= {value}",
                "gt" => $"input.{field} > {value}",
                "lte" => $"input.{field} <= {value}",
                "lt" => $"input.{field} < {value}",
                "eq" => $"input.{field} == {value}",
                "ne" => $"input.{field} != {value}",
                "in" => "", // Skip 'in' operator for now as it requires array handling
                _ => ""
            };
        }

        private class SimpleCondition
        {
            public string Type { get; set; } = "";
            public string Operator { get; set; } = "";
            public string Value { get; set; } = "";
        }
    }
}
