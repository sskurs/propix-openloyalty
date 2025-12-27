using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenLoyalty.Api.Data;
using OpenLoyalty.Api.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenLoyalty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly LoyaltyDbContext _context;

        public CampaignsController(LoyaltyDbContext context)
        {
            _context = context;
        }

        // GET: api/Campaigns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()
        {
            return await _context.Campaigns.AsNoTracking().ToListAsync();
        }

        // GET: api/Campaigns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Campaign>> GetCampaign(Guid id)
        {
            var campaign = await _context.Campaigns
                .Include(c => c.Conditions)
                .Include(c => c.Rewards)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (campaign == null) return NotFound();
            return campaign;
        }

        // POST: api/Campaigns
        [HttpPost]
        public async Task<ActionResult<Campaign>> PostCampaign([FromBody] CreateCampaignDto request)
        {
            if (request == null) return BadRequest("Request body is null");
            if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required");

            var campaign = new Campaign
            {
                Code = string.IsNullOrWhiteSpace(request.Code) ? "CAM-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper() : request.Code,
                Name = request.Name,
                Description = request.Description,
                Status = request.Status ?? "draft",
                StartAt = DateTime.SpecifyKind(request.StartAt, DateTimeKind.Utc),
                EndAt = request.EndAt.HasValue ? DateTime.SpecifyKind(request.EndAt.Value, DateTimeKind.Utc) : null,
                Priority = request.Priority,
                IsStackable = request.IsStackable,
                MaxTotalRewards = request.MaxTotalRewards,
                MaxPerCustomer = request.MaxPerCustomer,
                Type = request.Type
            };

            // Serialize RuleGroup if provided (new format), otherwise fall back to simple Conditions (legacy)
            string? ruleGroupJson = null;
            if (request.RuleGroup != null)
            {
                ruleGroupJson = JsonSerializer.Serialize(request.RuleGroup);
            }

            string? ruleEditorStateJson = null;
            if (request.RuleEditorState != null)
            {
                ruleEditorStateJson = JsonSerializer.Serialize(request.RuleEditorState);
            }

            if (request.Conditions != null && request.Conditions.Count > 0)
            {
                foreach (var c in request.Conditions)
                {
                    string valStr = "{}";
                    if (c.Value is string s) valStr = s;
                    else if (c.Value != null) valStr = JsonSerializer.Serialize(c.Value);

                    campaign.Conditions.Add(new CampaignCondition
                    {
                        ConditionType = c.Type ?? "unknown",
                        Operator = c.Operator ?? "eq",
                        Value = valStr,
                        RuleGroupJson = ruleGroupJson, // Store complex rules if provided
                        RuleEditorStateJson = ruleEditorStateJson ?? (c.RuleEditorState != null ? JsonSerializer.Serialize(c.RuleEditorState) : null)
                    });
                }
            }
            else if (ruleGroupJson != null)
            {
                // If only RuleGroup is provided (no simple conditions), create a single condition entry to store it
                campaign.Conditions.Add(new CampaignCondition
                {
                    ConditionType = "complex",
                    Operator = "rulegroup",
                    Value = "{}",
                    RuleGroupJson = ruleGroupJson,
                    RuleEditorStateJson = ruleEditorStateJson
                });
            }


            if (request.Rewards != null)
            {
                foreach (var r in request.Rewards)
                {
                    string valStr = "{}";
                    if (r.Value is string s) valStr = s;
                    else if (r.Value != null) valStr = JsonSerializer.Serialize(r.Value);

                    campaign.Rewards.Add(new CampaignReward
                    {
                        RewardType = r.Type ?? "unknown",
                        Value = valStr
                    });
                }
            }

            var outboxMessage = new OutboxMessage
            {
                Topic = "campaign.created",
                Key = campaign.Id.ToString(),
                Payload = JsonSerializer.Serialize(new
                {
                    type = "campaign.created",
                    payload = new
                    {
                        campaignId = campaign.Id,
                        code = campaign.Code,
                        name = campaign.Name,
                        description = campaign.Description,
                        status = campaign.Status,
                        priority = campaign.Priority,
                        isStackable = campaign.IsStackable,
                        startAt = campaign.StartAt,
                        endAt = campaign.EndAt,
                        conditions = request.Conditions,
                        ruleGroup = request.RuleGroup,
                        rewards = request.Rewards
                    }
                })
            };

            _context.Campaigns.Add(campaign);
            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCampaign), new { id = campaign.Id }, campaign);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampaign(Guid id, [FromBody] CreateCampaignDto request)
        {
            Console.WriteLine($"[PutCampaign] Start Update {id}");
            if (request == null) return BadRequest("Request body is null");

            var campaign = await _context.Campaigns.FindAsync(id);

            if (campaign == null) return NotFound();

            campaign.Code = string.IsNullOrWhiteSpace(request.Code) ? campaign.Code : request.Code;
            campaign.Name = request.Name ?? campaign.Name;
            campaign.Description = request.Description;
            campaign.Status = request.Status ?? campaign.Status;
            campaign.StartAt = DateTime.SpecifyKind(request.StartAt, DateTimeKind.Utc);
            campaign.EndAt = request.EndAt.HasValue ? DateTime.SpecifyKind(request.EndAt.Value, DateTimeKind.Utc) : null;
            campaign.Priority = request.Priority;
            campaign.IsStackable = request.IsStackable;
            campaign.MaxTotalRewards = request.MaxTotalRewards;
            campaign.MaxPerCustomer = request.MaxPerCustomer;
            campaign.Type = request.Type;
            campaign.UpdatedAt = DateTime.UtcNow;

            // Manual RowVersion update for Postgres (since [Timestamp] doesn't auto-update)
            campaign.RowVersion = Guid.NewGuid().ToByteArray();

            // Optimistic Concurrency Control
            if (request.RowVersion != null && request.RowVersion.Length > 0)
            {
                _context.Entry(campaign).Property(c => c.RowVersion).OriginalValue = request.RowVersion;
            }

            // Nuclear Option: Raw SQL Delete to bypass EF Core tracking entirely.
            // This avoids "0 rows affected" concurrency errors if the tracker gets confused.
            Console.WriteLine("[PutCampaign] Starting Transaction");
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Console.WriteLine("[PutCampaign] Executing Deletes");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM campaign_conditions WHERE \"CampaignId\" = {0}", id);
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM campaign_rewards WHERE \"CampaignId\" = {0}", id);
                Console.WriteLine("[PutCampaign] Deletes Complete");

            // Serialize RuleGroup if provided (new format), otherwise fall back to simple Conditions (legacy)
            string? ruleGroupJson = null;
            if (request.RuleGroup != null)
            {
                ruleGroupJson = JsonSerializer.Serialize(request.RuleGroup);
            }

            string? ruleEditorStateJson = null;
            if (request.RuleEditorState != null)
            {
                ruleEditorStateJson = JsonSerializer.Serialize(request.RuleEditorState);
            }

            if (request.Conditions != null && request.Conditions.Count > 0)
            {
                foreach (var c in request.Conditions)
                {
                    string valStr = "{}";
                    if (c.Value is string s) valStr = s;
                    else if (c.Value != null) valStr = JsonSerializer.Serialize(c.Value);

                    // DIRECT ADD to DbSet to avoid navigation property tracking issues
                    _context.CampaignConditions.Add(new CampaignCondition
                    {
                        CampaignId = campaign.Id,
                        ConditionType = c.Type ?? "unknown",
                        Operator = c.Operator ?? "eq",
                        Value = valStr,
                        RuleGroupJson = ruleGroupJson, // Store complex rules if provided
                        RuleEditorStateJson = ruleEditorStateJson ?? (c.RuleEditorState != null ? JsonSerializer.Serialize(c.RuleEditorState) : null)
                    });
                }
            }
            else if (ruleGroupJson != null)
            {
                // If only RuleGroup is provided (no simple conditions), create a single condition entry to store it
                _context.CampaignConditions.Add(new CampaignCondition
                {
                    CampaignId = campaign.Id,
                    ConditionType = "complex",
                    Operator = "rulegroup",
                    Value = "{}",
                    RuleGroupJson = ruleGroupJson,
                    RuleEditorStateJson = ruleEditorStateJson
                });
            }


            if (request.Rewards != null)
            {
                foreach (var r in request.Rewards)
                {
                    string valStr = "{}";
                    if (r.Value is string s) valStr = s;
                    else if (r.Value != null) valStr = JsonSerializer.Serialize(r.Value);

                    _context.CampaignRewards.Add(new CampaignReward
                    {
                        CampaignId = campaign.Id,
                        RewardType = r.Type ?? "unknown",
                        Value = valStr
                    });
                }
            }

            var outboxMessage = new OutboxMessage
            {
                Topic = "campaign.updated",
                Key = campaign.Id.ToString(),
                Payload = JsonSerializer.Serialize(new
                {
                    type = "campaign.updated",
                    payload = new
                    {
                        campaignId = campaign.Id,
                        code = campaign.Code,
                        name = campaign.Name,
                        description = campaign.Description,
                        status = campaign.Status,
                        priority = campaign.Priority,
                        isStackable = campaign.IsStackable,
                        startAt = campaign.StartAt,
                        endAt = campaign.EndAt,
                        conditions = request.Conditions,
                        ruleGroup = request.RuleGroup,
                        rewards = request.Rewards
                    }
                })
            };

            Console.WriteLine("[PutCampaign] Saving Changes");
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            Console.WriteLine("[PutCampaign] Committed");

            return Ok(new
            {
                id = campaign.Id,
                message = "Campaign updated successfully",
                rowVersion = campaign.RowVersion
            });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Conflict(new
                {
                    error = "CAMPAIGN_CONFLICT",
                    message = "This campaign was modified by another user.",
                    action = "RELOAD_REQUIRED"
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool CampaignExists(Guid id)
        {
            return _context.Campaigns.Any(e => e.Id == id);
        }

        // DELETE: api/Campaigns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampaign(Guid id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null) return NotFound();
            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
