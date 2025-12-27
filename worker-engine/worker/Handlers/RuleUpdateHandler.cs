using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;      // ✔ correct
using Worker.Repositories;
using Worker.Models;
using Worker.Infrastructure;
using Worker.Data;




namespace Worker.Handlers
{
  public class RuleUpdateHandler {
    private readonly WorkerDbContext _db; 
    private readonly ILogger<RuleUpdateHandler> _log;
    private readonly Worker.Engines.IAdvancedRuleEngine _engine;

    public RuleUpdateHandler(WorkerDbContext db, ILogger<RuleUpdateHandler> log, Worker.Engines.IAdvancedRuleEngine engine)
    { 
        _db=db; 
        _log=log; 
        _engine=engine;
    }

    public async Task<bool> HandleAsync(ConsumeResult<string,string> msg, CancellationToken ct){
      try{ 
        using var doc=JsonDocument.Parse(msg.Message.Value); 
        var payload=doc.RootElement.GetProperty("payload"); 
        var id=payload.GetProperty("id").GetString(); 
        var existing=await _db.EarningRules.FindAsync(new object[]{id}, ct);
        if(existing==null){ 
            var r=new EarningRule{ 
                Id=id ?? Guid.NewGuid().ToString(), 
                Name=payload.GetProperty("name").GetString() ?? "rule",
                ConditionJson = payload.TryGetProperty("conditionJson", out var cj) ? cj.GetString() : null,
                PointsJson = payload.TryGetProperty("pointsJson", out var pj) ? pj.GetString() : null,
                Status = "ACTIVE",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1
            }; 
            await _db.EarningRules.AddAsync(r, ct); 
        } else { 
            existing.Name=payload.GetProperty("name").GetString() ?? existing.Name; 
            existing.ConditionJson = payload.TryGetProperty("conditionJson", out var cj) ? cj.GetString() : existing.ConditionJson;
            existing.PointsJson = payload.TryGetProperty("pointsJson", out var pj) ? pj.GetString() : existing.PointsJson;
            existing.UpdatedAt = DateTime.UtcNow;
            _db.EarningRules.Update(existing); 
        }
        await _db.SaveChangesAsync(ct);
        
        // Update in-memory engine
        var conditionJson = payload.TryGetProperty("conditionJson", out var cj2) ? cj2.GetString() : null;
        var pointsJson = payload.TryGetProperty("pointsJson", out var pj2) ? pj2.GetString() : null;
        
        var ruleModel = new 
        {
            Id = id,
            Name = payload.GetProperty("name").GetString(),
            Conditions = !string.IsNullOrEmpty(conditionJson) ? JsonSerializer.Deserialize<object>(conditionJson) : null,
            Actions = !string.IsNullOrEmpty(pointsJson) ? JsonSerializer.Deserialize<object>(pointsJson) : null,
            IsActive = true,
            Version = 1
        };
        _engine.AddOrUpdateRule(id, ruleModel);
        
        return true;
      }catch(Exception ex){ _log.LogError(ex,"rule"); return false; }
    }
  }
}

