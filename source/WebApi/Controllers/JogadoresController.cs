using FSP.Api.Infrastructure.Data.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    public class JogadoresController(ApplicationDbContext db, IConfiguration configuration) : ControllerBase
    {
        private string PlayerPhotosPath =>
            configuration["FileStorage:PlayerPhotosPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads", "players");

        // ── POST /api/v1/jogadores/{playerId}/foto ────────────────────────────
        // Somente admin.

        [HttpPost("api/v1/jogadores/{playerId}/foto")]
        [Authorize]
        public async Task<IActionResult> UploadFoto(string playerId, IFormFile foto, [FromForm] string? playerName)
        {
            if (!User.IsInRole("Administrador")) return Forbid();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(foto.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = $"Formato inválido. Aceitos: {string.Join(", ", allowedExtensions)}" });

            if (foto.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Tamanho máximo: 5 MB." });

            if (!Directory.Exists(PlayerPhotosPath))
                Directory.CreateDirectory(PlayerPhotosPath);

            var fileName = $"{playerId}{extension}";
            var fullPath = Path.Combine(PlayerPhotosPath, fileName);

            // Remove arquivo anterior de extensão diferente
            foreach (var ext in allowedExtensions)
            {
                var old = Path.Combine(PlayerPhotosPath, $"{playerId}{ext}");
                if (old != fullPath && System.IO.File.Exists(old))
                    System.IO.File.Delete(old);
            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await foto.CopyToAsync(stream);

            var existing = await db.JogadoresFotos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(j => j.PlayerId == playerId);

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? userId = Guid.TryParse(userIdStr, out var uid) ? uid : null;

            if (existing != null)
            {
                existing.NomeArquivo = fileName;
                existing.PlayerName = playerName ?? existing.PlayerName;
                existing.AtualizadoPorUserId = userId;
                existing.DataModificacao = DateTimeOffset.UtcNow;
                existing.Excluido = false;
            }
            else
            {
                db.JogadoresFotos.Add(new Domain.Entities.JogadorFoto.JogadorFoto
                {
                    Id = Guid.NewGuid(),
                    PlayerId = playerId,
                    PlayerName = playerName ?? playerId,
                    NomeArquivo = fileName,
                    AtualizadoPorUserId = userId,
                    CriadoEm = DateTimeOffset.UtcNow,
                    DataModificacao = DateTimeOffset.UtcNow,
                });
            }

            await db.SaveChangesAsync();
            return Ok(new { status = "success", nomeArquivo = fileName });
        }

        // ── GET /api/v1/jogadores/fotos?ids=1,2,3 ────────────────────────────
        // Público.

        [HttpGet("api/v1/jogadores/fotos")]
        public async Task<IActionResult> GetFotos([FromQuery] string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return Ok(new { status = "success", fotos = new { } });

            var playerIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .Distinct()
                               .ToList();

            var registros = await db.JogadoresFotos
                .Where(j => playerIds.Contains(j.PlayerId))
                .Select(j => new { j.PlayerId, j.NomeArquivo })
                .ToListAsync();

            var fotos = new Dictionary<string, object>();

            foreach (var r in registros)
            {
                var fullPath = Path.Combine(PlayerPhotosPath, r.NomeArquivo);
                if (!System.IO.File.Exists(fullPath)) continue;

                var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                var ext = Path.GetExtension(r.NomeArquivo).ToLower();
                var contentType = ext switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                fotos[r.PlayerId] = new
                {
                    base64 = Convert.ToBase64String(bytes),
                    contentType,
                };
            }

            return Ok(new { status = "success", fotos });
        }
    }
}
