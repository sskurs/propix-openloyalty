using Worker.Models; // For RuleAction

namespace Worker.Strategies
{
    public interface IRewardStrategy
    {
        // Evaluates if this strategy can handle the given action type
        bool CanHandle(string actionType);

        // Executes the reward logic
        Task ExecuteAsync(RuleAction action, string userId, string txId, string ruleId);
    }
}
