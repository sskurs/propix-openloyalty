using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Worker.Engines
{
    /// <summary>
    /// Advanced in-memory rule engine that stores rules as JSON and evaluates them.
    /// Rule JSON model documented below.
    /// </summary>
    public class AdvancedRuleEngine : IAdvancedRuleEngine
    {
        // store raw json per ruleId
        private readonly ConcurrentDictionary<string, string> _rulesJson = new();

        // --- IRuleEngine implementation ---

        public void AddOrUpdateRule(string ruleId, object ruleData)
        {
            var json = Serialize(ruleData);
            _rulesJson[ruleId] = json;
        }

        public object? GetRule(string ruleId)
        {
            if (!_rulesJson.TryGetValue(ruleId, out var json)) return null;
            return JsonSerializer.Deserialize<RuleModel>(json, JsonOptions);
        }

        // --- Advanced evaluation API ---

        public async Task<EvaluationResult> EvaluateAsync(string ruleId, IDictionary<string, object?> context)
        {
            if (!_rulesJson.TryGetValue(ruleId, out var json))
                return new EvaluationResult { Matched = false };

            var model = JsonSerializer.Deserialize<RuleModel>(json, JsonOptions);
            if (model == null) return new EvaluationResult { Matched = false };

            // Synchronous evaluation
            Console.WriteLine($"[RuleEngine] Evaluating Rule {ruleId} with context keys: {string.Join(", ", context.Keys)}");
            var matched = EvaluateConditions(model.Conditions, context, out var logMsg);

            if (!matched)
            {
               Console.WriteLine($"[RuleEngine] Rule {ruleId} MATCH FAILED: {logMsg}");
            }
            else
            {
               Console.WriteLine($"[RuleEngine] Rule {ruleId} MATCH SUCCESS");
            }

            var result = new EvaluationResult { Matched = matched };
            if (matched && model.Actions != null)
            {
                foreach (var a in model.Actions)
                {
                    var res = new RuleActionResult
                    {
                        ActionType = a.Type,
                        Parameters = a.Parameters != null
                            ? a.Parameters.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value)
                            : new Dictionary<string, object?>()
                    };

                    result.Actions.Add(res);
                }
            }

            return await Task.FromResult(result);
        }

        public IEnumerable<string> GetAllRuleIds()
        {
            return _rulesJson.Keys;
        }

        // --- Helpers ---

        private static bool EvaluateConditions(IList<ConditionModel>? conditions, IDictionary<string, object?> context, out string logMsg)
        {
            logMsg = "";
            if (conditions == null || conditions.Count == 0) return true; // empty = always match

            // default combinator is AND
            foreach (var c in conditions)
            {
                var ok = EvaluateCondition(c, context, out var condLog);
                if (!(c.Combinator?.Equals("AND", StringComparison.OrdinalIgnoreCase) ?? true))
                {
                    if (c.Combinator?.Equals("OR", StringComparison.OrdinalIgnoreCase) ?? false)
                    {
                        if (ok) return true;
                    }
                    else
                    {
                        // unknown combinator -> treat as AND
                        if (!ok) { logMsg = condLog; return false; }
                    }
                }
                else
                {
                    if (!ok) { logMsg = condLog; return false; }
                }
            }

            return true;
        }

        private static bool EvaluateCondition(ConditionModel c, IDictionary<string, object?> context, out string logMsg)
        {
            logMsg = "";
            if (!context.TryGetValue(c.Field ?? "", out var ctxValue))
            {
                logMsg = $"Field '{c.Field}' not found in context. Available: {string.Join(", ", context.Keys)}";
                return false;
            }

            bool result = false;
            switch (c.Operator?.Trim().ToLowerInvariant())
            {
                case "equals":
                case "==":
                case "eq":
                    result = EqualsOp(ctxValue, c.Value);
                    break;
                case "gt":
                case ">":
                case "greaterthan":
                    result = CompareOp(ctxValue, c.Value, (a, b) => a > b);
                    break;
                case "lt":
                case "<":
                case "lessthan":
                    result = CompareOp(ctxValue, c.Value, (a, b) => a < b);
                    break;
                case "gte":
                case ">=":
                case "greaterthanorequal":
                    result = CompareOp(ctxValue, c.Value, (a, b) => a >= b);
                    break;
                case "lte":
                case "<=":
                case "lessthanorequal":
                    result = CompareOp(ctxValue, c.Value, (a, b) => a <= b);
                    break;
                case "contains":
                    result = ContainsOp(ctxValue, c.Value);
                    break;
                case "in":
                    result = InOp(ctxValue, c.Value);
                    break;
                default:
                    logMsg = $"Unknown operator '{c.Operator}'";
                    return false;
            }

            if (!result) logMsg = $"Condition Failed: {c.Field} ({ctxValue}) {c.Operator} {c.Value}";
            return result;
        }

        private static bool EqualsOp(object? left, JsonElement? rightElem)
        {
            if (rightElem == null) return left == null;
            var right = ExtractValue(rightElem.Value);

            if (left == null) return right == null;
            return left.Equals(right);
        }

        private static bool CompareOp(object? left, JsonElement? rightElem, Func<decimal, decimal, bool> cmp)
        {
            if (rightElem == null) return false;
            var right = ExtractValue(rightElem.Value);

            if (!TryToDecimal(left, out var ld)) return false;
            if (!TryToDecimal(right, out var rd)) return false;

            return cmp(ld, rd);
        }

        private static bool ContainsOp(object? left, JsonElement? rightElem)
        {
            if (left == null || rightElem == null) return false;
            var right = ExtractValue(rightElem.Value);
            if (left is string s && right is string r) return s.Contains(r, StringComparison.OrdinalIgnoreCase);
            if (left is System.Collections.IEnumerable enumerable && right != null)
            {
                foreach (var item in enumerable) if (item?.Equals(right) ?? right == null) return true;
            }
            return false;
        }

        private static bool InOp(object? left, JsonElement? rightElem)
        {
            if (rightElem == null) return false;
            if (rightElem.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var el in rightElem.Value.EnumerateArray())
                {
                    var val = ExtractValue(el);
                    if (left?.Equals(val) ?? val == null) return true;
                }
            }
            return false;
        }

        private static object? ExtractValue(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Number when el.TryGetInt64(out var v) => v,
                JsonValueKind.Number when el.TryGetDecimal(out var dec) => dec,
                JsonValueKind.String => el.GetString(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => el.GetRawText()
            };
        }

        private static bool TryToDecimal(object? o, out decimal d)
        {
            d = 0;
            if (o == null) return false;
            if (o is decimal dm) { d = dm; return true; }
            if (o is double dbl) { d = Convert.ToDecimal(dbl); return true; }
            if (o is float f) { d = Convert.ToDecimal(f); return true; }
            if (o is int i) { d = i; return true; }
            if (o is long l) { d = l; return true; }
            if (o is string s && decimal.TryParse(s, out var p)) { d = p; return true; }
            return false;
        }

        private static string Serialize(object obj) =>
            JsonSerializer.Serialize(obj, JsonOptions);

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        // --- Rule JSON model types ---

        private class RuleModel
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public IList<ConditionModel>? Conditions { get; set; }
            public IList<ActionModel>? Actions { get; set; }
            public bool IsActive { get; set; } = true;
            public int Version { get; set; }
        }

        private class ConditionModel
        {
            public string? Field { get; set; }            // context key, e.g. "order.total"
            public JsonElement? Value { get; set; }       // value to compare (kept as JsonElement for flexibility)
            public string? Operator { get; set; }         // equals, gt, lt, contains, in
            public string? Combinator { get; set; }       // AND / OR (simple handling)
        }

        private class ActionModel
        {
            public string? Type { get; set; }             // addPoints, setTier, enqueueJob, etc.
            public Dictionary<string, JsonElement>? Parameters { get; set; } // action parameters
        }
    }
}