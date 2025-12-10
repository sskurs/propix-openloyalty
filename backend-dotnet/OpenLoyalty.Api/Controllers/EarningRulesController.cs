using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [Route("api/earningrules")] // CORRECTED: from "api/earning-rules"
    [ApiController]
    public class EarningRulesController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public EarningRulesController(LoyaltyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EarningRule>>> GetEarningRules()
        {
            return await _context.EarningRules.AsNoTracking().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EarningRule>> GetEarningRule(string id)
        {
            var rule = await _context.EarningRules.FindAsync(id);
            if (rule == null) return NotFound();
            return rule;
        }

        [HttpPost]
        public async Task<ActionResult<EarningRule>> PostEarningRule(CreateEarningRuleDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newRule = new EarningRule
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Status = createDto.Status,
                Category = createDto.Category,
                EventKey = createDto.EventKey,
                Priority = createDto.Priority,
                ConditionJson = JsonSerializer.Serialize(createDto.Condition),
                PointsJson = JsonSerializer.Serialize(createDto.Points),
                LimitsJson = createDto.Limits == null ? null : JsonSerializer.Serialize(createDto.Limits),
                TimeWindowJson = createDto.TimeWindow == null ? null : JsonSerializer.Serialize(createDto.TimeWindow),
                SegmentsJson = createDto.Segments == null ? null : JsonSerializer.Serialize(createDto.Segments),
                ActivateAt = createDto.ActivateAt,
                DeactivateAt = createDto.DeactivateAt,
                CronExpression = createDto.CronExpression,
            };

            var outboxPayload = new
            {
                id = newRule.Id,
                name = newRule.Name,
                description = newRule.Description,
                active = newRule.Status == "ACTIVE",
                priority = newRule.Priority,
                version = newRule.Version,
                type = newRule.Type,
                condition = createDto.Condition,
                points = createDto.Points,
                limits = createDto.Limits,
                timeWindow = createDto.TimeWindow,
                segments = createDto.Segments,
                metadata = (object)null,
                createdAt = newRule.CreatedAt,
                updatedAt = newRule.UpdatedAt
            };

            var outboxMessage = new OutboxMessage
            {
                Topic = "earning-rule.created",
                Payload = JsonSerializer.Serialize(outboxPayload, new JsonSerializerOptions 
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            };

            _context.EarningRules.Add(newRule);
            _context.OutboxMessages.Add(outboxMessage);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEarningRule), new { id = newRule.Id }, newRule);
        }
    }
}
