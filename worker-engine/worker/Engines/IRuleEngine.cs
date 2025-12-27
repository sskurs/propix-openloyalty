namespace Worker.Engines
{
    public interface IRuleEngine
    {
        void AddOrUpdateRule(string ruleId, object ruleData);
        object? GetRule(string ruleId);
    }
}
