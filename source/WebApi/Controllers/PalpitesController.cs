using FSP.Api.Infrastructure.Data.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    public class PalpitesController(ApplicationDbContext db) : ControllerBase
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

            var json = JsonSerializer.Serialize(new
            {
                status = "success",
                response = new { palpites, total = palpites.Count }
            });
            return Content(json, "application/json");
        }
    }
}
