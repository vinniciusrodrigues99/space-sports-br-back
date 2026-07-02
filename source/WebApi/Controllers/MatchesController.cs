using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    public class MatchesController(IHttpClientFactory httpClientFactory, IMemoryCache cache) : ControllerBase
    {
        private static TimeSpan GetCacheTtl(string dateStr)
        {
            var brtToday = DateTimeOffset.UtcNow
                .ToOffset(TimeSpan.FromHours(-3))
                .ToString("yyyy-MM-dd");
            return DateOnly.Parse(dateStr) < DateOnly.Parse(brtToday)
                ? TimeSpan.FromHours(24)
                : TimeSpan.FromHours(12);
        }

        // ── GET /api/v1/matches ───────────────────────────────────────────────

        [HttpGet("api/v1/matches")]
        public async Task<IActionResult> GetByDate([FromQuery] string? date, [FromQuery] int? leagueId)
        {
            var dateStr = ResolveDate(date);
            var cacheKey = $"matches_{dateStr}";
            if (cache.TryGetValue(cacheKey, out string? cached))
                return Content(cached!, "application/json");

            var filtered = await GetFilteredMatchesAsync(dateStr);
            var json = JsonSerializer.Serialize(new
            {
                status = "success",
                response = new { matches = filtered }
            });

            if (filtered.Count > 0)
                cache.Set(cacheKey, json, GetCacheTtl(dateStr));
            return Content(json, "application/json");
        }

        // ── GET /api/v1/players?date=YYYY-MM-DD ───────────────────────────────
        // Busca o elenco completo (26 jogadores) de cada seleção via roster ESPN.
        // Funciona antes do anúncio da escalação, pois usa o elenco convocado fixo.

        [HttpGet("api/v1/players")]
        public async Task<IActionResult> GetAllPlayers([FromQuery] string? date)
        {
            var dateStr = ResolveDate(date);
            var cacheKey = $"all_players_{dateStr}";
            if (cache.TryGetValue(cacheKey, out string? cached))
                return Content(cached!, "application/json");

            var matches = await GetFilteredMatchesAsync(dateStr);

            // Extrai (matchId, teamId, teamName, side) de todos os jogos do dia
            var teamRefs = matches.SelectMany(m =>
            {
                var matchId = m["id"]?.GetValue<string>() ?? "";
                var comp = (m["competitions"] as JsonArray)?.FirstOrDefault();
                if (comp == null || string.IsNullOrEmpty(matchId)) return [];
                return (comp["competitors"] as JsonArray)
                    ?.OfType<JsonNode>()
                    .Select(c => (
                        matchId,
                        teamId: c["team"]?["id"]?.GetValue<string>() ?? "",
                        teamName: c["team"]?["displayName"]?.GetValue<string>() ?? "",
                        side: c["homeAway"]?.GetValue<string>() ?? "home"
                    ))
                    .Where(t => !string.IsNullOrEmpty(t.teamId))
                    ?? [];
            }).ToList();

            var client = httpClientFactory.CreateClient("espn");
            var rosterBatches = await Task.WhenAll(
                teamRefs.Select(t => FetchTeamRosterAsync(client, t.teamId, t.teamName, t.side, t.matchId))
            );

            var allPlayers = rosterBatches.SelectMany(p => p).ToList();
            var json = JsonSerializer.Serialize(new
            {
                status = "success",
                response = new { players = allPlayers }
            });

            if (allPlayers.Count > 0)
                cache.Set(cacheKey, json, GetCacheTtl(dateStr));
            return Content(json, "application/json");
        }

        // ── GET /api/v1/players/{playerId}/image ──────────────────────────────
        // Redireciona para ESPN CDN — sem proxy, sem cópia de bytes.

        [HttpGet("api/v1/players/{playerId}/image")]
        public IActionResult GetPlayerImage(int playerId)
        {
            return Redirect($"https://a.espncdn.com/i/headshots/soccer/players/full/{playerId}.png");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string ResolveDate(string? date) =>
            string.IsNullOrWhiteSpace(date)
                ? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-3)).ToString("yyyy-MM-dd")
                : date;

        private async Task<List<JsonNode>> GetFilteredMatchesAsync(string dateStr)
        {
            // ESPN usa EDT (UTC-4) para bucketing. Jogo às 03:00Z do dia D é
            // 23:00 EDT do dia D-1 → bucket ESPN = D-1. Buscamos D-1, D e D+1
            // para garantir cobertura total, depois filtramos pela data BRT pedida.
            var date = DateOnly.Parse(dateStr);
            var prevDateStr = date.AddDays(-1).ToString("yyyy-MM-dd");
            var nextDateStr = date.AddDays(1).ToString("yyyy-MM-dd");
            var client = httpClientFactory.CreateClient("espn");
            var results = await Task.WhenAll(
                FetchRawAsync(client, prevDateStr),
                FetchRawAsync(client, dateStr),
                FetchRawAsync(client, nextDateStr)
            );

            var seen = new HashSet<string>();
            return results
                .SelectMany(list => list)
                .Where(m =>
                {
                    var id = m["id"]?.GetValue<string>() ?? "";
                    if (!seen.Add(id)) return false;

                    // Converte horário UTC do ESPN para BRT (UTC-3) e filtra pela data pedida
                    var dateVal = m["date"]?.GetValue<string>();
                    if (string.IsNullOrEmpty(dateVal)) return false;
                    if (!DateTimeOffset.TryParse(dateVal, out var dto)) return false;
                    var brtDate = dto.ToOffset(TimeSpan.FromHours(-3)).ToString("yyyy-MM-dd");
                    return brtDate == dateStr;
                })
                .ToList();
        }

        private static async Task<IEnumerable<JsonNode>> FetchRawAsync(HttpClient client, string dateStr)
        {
            var d = dateStr.Replace("-", "");
            try
            {
                var response = await client.GetAsync(
                    $"apis/site/v2/sports/soccer/fifa.world/scoreboard?dates={d}");
                if (!response.IsSuccessStatusCode) return [];
                var content = await response.Content.ReadAsStringAsync();
                var node = JsonNode.Parse(content);
                return (node?["events"] as JsonArray)?.OfType<JsonNode>() ?? [];
            }
            catch { return []; }
        }

        private async Task<IEnumerable<object>> FetchTeamRosterAsync(
            HttpClient client, string teamId, string teamName, string side, string matchId)
        {
            var rosterCacheKey = $"roster_{teamId}";
            if (cache.TryGetValue(rosterCacheKey, out IEnumerable<object>? cachedRoster))
                return cachedRoster!;

            try
            {
                var response = await client.GetAsync(
                    $"apis/site/v2/sports/soccer/fifa.world/teams/{teamId}/roster");
                if (!response.IsSuccessStatusCode) return [];
                var content = await response.Content.ReadAsStringAsync();
                var node = JsonNode.Parse(content);

                // ESPN roster pode vir como lista plana ou agrupada por posição
                var athletes = node?["athletes"] as JsonArray;
                if (athletes == null) return [];

                var players = new List<object>();

                // Detecta se é lista agrupada ({items: [...]}) ou lista plana de athletes
                var firstItem = athletes.FirstOrDefault();
                var isGrouped = firstItem?["items"] != null;

                IEnumerable<JsonNode?> flatAthletes = isGrouped
                    ? athletes.OfType<JsonNode>()
                        .SelectMany(g => (g["items"] as JsonArray)?.OfType<JsonNode>() ?? [])
                    : athletes.OfType<JsonNode>();

                foreach (var entry in flatAthletes)
                {
                    if (entry == null) continue;

                    // Estrutura varia: pode ser {athlete: {...}} ou direto o athlete
                    var athleteNode = entry["athlete"] ?? entry;

                    var idStr = athleteNode["id"]?.GetValue<string>();
                    if (!int.TryParse(idStr, out var id)) continue;

                    var name = athleteNode["displayName"]?.GetValue<string>()
                             ?? athleteNode["fullName"]?.GetValue<string>();
                    if (string.IsNullOrWhiteSpace(name)) continue;

                    var jersey = entry["jersey"]?.GetValue<string>()
                               ?? athleteNode["jersey"]?.GetValue<string>();
                    var positionNode = entry["position"] ?? athleteNode["position"];
                    var position = positionNode?["abbreviation"]?.GetValue<string>();

                    var imageUrl = athleteNode["headshot"]?["href"]?.GetValue<string>()
                                ?? $"https://img.fifa.com/image/upload/players/{id}.png";

                    players.Add(new
                    {
                        id,
                        name,
                        shortName = athleteNode["shortName"]?.GetValue<string>(),
                        shirt = int.TryParse(jersey, out var s) ? s : (int?)null,
                        position,
                        imageUrl,
                        teamName,
                        teamId = int.TryParse(teamId, out var tid) ? tid : (int?)null,
                        matchId = int.TryParse(matchId, out var mid) ? mid : (int?)null,
                        side,
                        isStarter = (bool?)null,
                    });
                }

                // Elenco convocado é fixo durante o torneio — cache de 24h
                if (players.Count > 0)
                    cache.Set(rosterCacheKey, (IEnumerable<object>)players, TimeSpan.FromHours(24));

                return players;
            }
            catch { return []; }
        }
    }
}
