using Newtonsoft.Json.Linq;

public class JsonRuleEngine : IRuleEngine {

    public bool EvaluateRule(string conditionJson, JObject eventPayload, UserSnapshot snapshot) {
        var root = JToken.Parse(conditionJson);
        return EvalNode(root, eventPayload, snapshot);
    }

    private bool EvalNode(JToken node, JObject evt, UserSnapshot snap) {
        if (node["operator"] != null) {
            var op = node["operator"]!.ToString().ToLower();
            var rules = node["rules"]!.Children();

            return op == "or"
                ? rules.Any(r => EvalNode(r, evt, snap))
                : rules.All(r => EvalNode(r, evt, snap));
        }

        var field = node["field"]!.ToString();
        var op2 = node["op"]?.ToString() ?? "eq";
        var right = node["value"];

        var left = Resolve(field, evt, snap);
        return Compare(left, op2, right);
    }

    private object? Resolve(string field, JObject evt, UserSnapshot snap) =>
        field switch {
            "TotalSpent" => snap.TotalSpent,
            "OrdersCount" => snap.OrdersCount,
            "LastPurchaseDays" =>
                snap.LastPurchaseAt.HasValue
                    ? (DateTime.UtcNow - snap.LastPurchaseAt.Value).Days
                    : int.MaxValue,
            _ when field.StartsWith("event:") =>
                evt.SelectToken(field.Substring(6))?.ToString(),
            _ => null
        };

    private bool Compare(object? left, string op, JToken? right) {
        if (left == null) return false;

        if (decimal.TryParse(left.ToString(), out var l)
            && decimal.TryParse(right?.ToString(), out var r)) {

            return op switch {
                "gt" => l > r,
                "gte" => l >= r,
                "lt" => l < r,
                "lte" => l <= r,
                "eq" => l == r,
                "neq" => l != r,
                _ => false
            };
        }

        return op switch {
            "eq" => left.ToString() == right?.ToString(),
            "neq" => left.ToString() != right?.ToString(),
            "contains" => left.ToString()?.Contains(right?.ToString() ?? "") ?? false,
            _ => false
        };
    }

    public int ComputePoints(string pointsJson, JObject eventPayload, UserSnapshot snapshot) {
        var p = JObject.Parse(pointsJson);
        var type = p["type"]!.ToString();

        if (type == "per_amount") {
            decimal amount = (decimal)p["amount"]!;
            int pts = (int)p["points"]!;
            var gross = eventPayload.SelectToken("payload.gross_value")!.Value<decimal>();
            return (int)(gross / amount) * pts;
        }

        if (type == "flat")
            return (int)p["points"]!;

        return 0;
    }
}
