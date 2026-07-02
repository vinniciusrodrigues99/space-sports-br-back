using FSP.Api.Infrastructure.Data.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    public class PalpitesController(ApplicationDbContext db, IHttpClientFactory httpClientFactory, IMemoryCache cache) : ControllerBase
    {
        public record PalpiteRequest(
            int EventId,
            string HomeName,
            string AwayName,
            int HomeGoals,
            int AwayGoals,
            string Nickname,
            string? Stage
        );

        // ── POST /api/v1/palpites ─────────────────────────────────────────────
        // Público — qualquer visitante pode enviar. Reenvio sobrescreve.

        [HttpPost("api/v1/palpites")]
        public async Task<IActionResult> Save([FromBody] PalpiteRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Nickname)) return BadRequest();

            var nick = req.Nickname.Trim();
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var uid) ? uid : null;

            // Busca palpite existente do mesmo nickname no mesmo jogo
            var existing = await db.Palpites
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.EventId == req.EventId &&
                                          p.Nickname.ToLower() == nick.ToLower() &&
                                          !p.Excluido);

            if (existing != null)
            {
                existing.HomeGoals = Math.Max(0, req.HomeGoals);
                existing.AwayGoals = Math.Max(0, req.AwayGoals);
                existing.HomeName = req.HomeName ?? "";
                existing.AwayName = req.AwayName ?? "";
                existing.DataModificacao = DateTimeOffset.UtcNow;
                if (userId.HasValue) existing.UserId = userId;
            }
            else
            {
                db.Palpites.Add(new Domain.Entities.Palpite.Palpite
                {
                    Id = Guid.NewGuid(),
                    EventId = req.EventId,
                    HomeName = req.HomeName ?? "",
                    AwayName = req.AwayName ?? "",
                    HomeGoals = Math.Max(0, req.HomeGoals),
                    AwayGoals = Math.Max(0, req.AwayGoals),
                    Nickname = nick,
                    UserId = userId,
                    Stage = req.Stage,
                    CriadoEm = DateTimeOffset.UtcNow,
                    DataModificacao = DateTimeOffset.UtcNow,
                });
            }

            await db.SaveChangesAsync();
            return Ok(new { status = "success" });
        }

        // ── GET /api/v1/palpites ──────────────────────────────────────────────
        // Somente admin.

        [HttpGet("api/v1/palpites")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] int? eventId)
        {
            if (!User.IsInRole("Administrador")) return Forbid();

            var query = db.Palpites.AsQueryable();
            if (eventId.HasValue)
                query = query.Where(p => p.EventId == eventId.Value);

            var palpites = await query
                .OrderByDescending(p => p.CriadoEm)
                .Select(p => new
                {
                    eventId = p.EventId,
                    homeName = p.HomeName,
                    awayName = p.AwayName,
                    homeGoals = p.HomeGoals,
                    awayGoals = p.AwayGoals,
                    nickname = p.Nickname,
                    stage = p.Stage,
                    submittedAt = p.CriadoEm,
                    userPhotoUrl = p.UserId != null
                        ? db.Users.Where(u => u.Id == p.UserId).Select(u => u.TxFoto).FirstOrDefault()
                        : null,
                })
                .ToListAsync();

            // Fetch actual scores from ESPN for each unique eventId
            var actualScores = new Dictionary<int, (int Home, int Away)?>();
            var client = httpClientFactory.CreateClient();
            foreach (var eid in palpites.Select(p => p.eventId).Distinct())
            {
                var cacheKey = $"actual_score_{eid}";
                if (cache.TryGetValue(cacheKey, out (int, int)? cached))
                {
                    actualScores[eid] = cached;
                    continue;
                }
                try
                {
                    var summaryJson = await client.GetStringAsync(
                        $"https://site.api.espn.com/apis/site/v2/sports/soccer/fifa.world/summary?event={eid}");
                    var doc = JsonNode.Parse(summaryJson);
                    var comp = doc?["header"]?["competitions"]?[0];
                    var completed = comp?["status"]?["type"]?["completed"]?.GetValue<bool>() ?? false;
                    if (!completed) { actualScores[eid] = null; continue; }

                    var competitors = comp?["competitors"]?.AsArray() ?? [];
                    var home = competitors.FirstOrDefault(c => c?["homeAway"]?.GetValue<string>() == "home");
                    var away = competitors.FirstOrDefault(c => c?["homeAway"]?.GetValue<string>() == "away");
                    if (home == null || away == null) { actualScores[eid] = null; continue; }

                    var hs = int.TryParse(home["score"]?.ToString(), out int hv) ? hv : (int?)null;
                    var as_ = int.TryParse(away["score"]?.ToString(), out int av) ? av : (int?)null;

                    var score = (hs.HasValue && as_.HasValue) ? ((int, int)?)(hs.Value, as_.Value) : null;
                    actualScores[eid] = score;
                    cache.Set(cacheKey, score, TimeSpan.FromHours(2));
                }
                catch { actualScores[eid] = null; }
            }

            var enriched = palpites.Select(p =>
            {
                var actual = actualScores.TryGetValue(p.eventId, out var s) ? s : null;
                return new
                {
                    p.eventId, p.homeName, p.awayName,
                    p.homeGoals, p.awayGoals,
                    p.nickname, p.stage, p.submittedAt, p.userPhotoUrl,
                    actualHomeGoals = actual?.Home,
                    actualAwayGoals = actual?.Away,
                };
            }).ToList();

            var json = JsonSerializer.Serialize(new
            {
                status = "success",
                response = new { palpites = enriched, total = enriched.Count }
            });
            return Content(json, "application/json");
        }
    }
}
