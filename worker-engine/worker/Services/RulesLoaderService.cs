using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Worker.Data;
using Worker.Engines;
using Worker.Models;

namespace Worker.Services
{
    public class RulesLoaderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RulesLoaderService> _logger;
        private readonly IRuleEngine _ruleEngine;

        public RulesLoaderService(IServiceProvider serviceProvider, ILogger<RulesLoaderService> logger, IRuleEngine ruleEngine)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _ruleEngine = ruleEngine;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RulesLoaderService starting hydration...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<WorkerDbContext>();

                try
                {
                    var allCampaigns = await db.Campaigns.ToListAsync(stoppingToken);
                    foreach(var c in allCampaigns) 
                    {
                        _logger.LogInformation("Found Campaign: {Id}, Code: {Code}, Status: {Status}", c.Id, c.Code, c.Status);
                    }

                    var campaigns = allCampaigns
                        .Where(c => c.Status != null && c.Status.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    _logger.LogInformation("Found {Count} active campaigns to load.", campaigns.Count);

                    foreach (var camp in campaigns)
                    {
                        try
                        {
                            var cid = Guid.Parse(camp.Id);

                            var condEntity = await db.CampaignConditions.FirstOrDefaultAsync(x => x.CampaignId == cid, stoppingToken);
                            if (condEntity != null && !string.IsNullOrWhiteSpace(condEntity.ConditionJson))
                            {
                                camp.Conditions = JsonSerializer.Deserialize<List<CampaignConditionModel>>(condEntity.ConditionJson, JsonOptions);
                            }

                            var rewEntity = await db.CampaignRewards.FirstOrDefaultAsync(x => x.CampaignId == cid, stoppingToken);
                            if (rewEntity != null && !string.IsNullOrWhiteSpace(rewEntity.RewardJson))
                            {
                                camp.Rewards = JsonSerializer.Deserialize<List<CampaignRewardModel>>(rewEntity.RewardJson, JsonOptions);
                            }

                            var ruleModel = new
                            {
                                Id = camp.Id,
                                Name = camp.Name,
                                Conditions = camp.Conditions?.Select(c => MapCondition(c)).ToList() ?? new List<object>(),
                                Actions = camp.Rewards?.Select(r => MapReward(r)).ToList() ?? new List<object>(),
                                IsActive = true
                            };

                            _ruleEngine.AddOrUpdateRule(camp.Id, ruleModel);
                            _logger.LogInformation("Hydrated Campaign {Id}", camp.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to hydrate campaign {Id}", camp.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading rules from DB");
                }
            }
        }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private object MapCondition(CampaignConditionModel c)
        {
            return new
            {
                Field = c.Type,
                Operator = c.Operator,
                Value = ExtractConditionValue(c.Value),
                Combinator = "AND"
            };
        }

        private object? ExtractConditionValue(object? val)
        {
            string? s = null;
            if (val is JsonElement je)
            {
                if (je.ValueKind == JsonValueKind.String) s = je.GetString();
                else s = je.GetRawText();
            }
            else if (val is string str) s = str;

            if (string.IsNullOrEmpty(s)) return null;

            try
            {
                using var doc = JsonDocument.Parse(s);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("value", out var v))
                {
                    return MapJsonElement(v);
                }
                return MapJsonElement(root);
            }
            catch
            {
                return s;
            }
        }

        private object? MapJsonElement(JsonElement el)
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

        private object MapReward(CampaignRewardModel r)
        {
            var paramsDict = new Dictionary<string, object?>();
            if (r.Value != null)
            {
                string? json = null;
                if (r.Value is JsonElement je) json = je.ValueKind == JsonValueKind.String ? je.GetString() : je.GetRawText();
                else if (r.Value is string s) json = s;

                if (!string.IsNullOrWhiteSpace(json) && json.Trim().StartsWith("{"))
                {
                     try
                     {
                         var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                         if (dict != null) foreach(var kv in dict) paramsDict[kv.Key] = kv.Value;
                     }
                     catch { }
                }
            }
            return new { Type = r.Type, Parameters = paramsDict };
        }
    }
}
