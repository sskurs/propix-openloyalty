using Newtonsoft.Json.Linq;

public interface IRuleEngine {
    bool EvaluateRule(string conditionJson, JObject eventPayload, UserSnapshot snapshot);
    int ComputePoints(string pointsJson, JObject eventPayload, UserSnapshot snapshot);
}
