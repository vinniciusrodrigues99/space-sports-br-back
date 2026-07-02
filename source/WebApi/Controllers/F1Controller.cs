using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    public class F1Controller(IHttpClientFactory httpClientFactory, IMemoryCache cache) : ControllerBase
    {
        // ── GET /api/v1/f1/podium ─────────────────────────────────────────────

        [HttpGet("api/v1/f1/podium")]
        public async Task<IActionResult> GetPodium()
        {
            const string cacheKey = "f1_podium";
            if (cache.TryGetValue(cacheKey, out string? cached))
                return Content(cached!, "application/json");

            var client = httpClientFactory.CreateClient();

            // 1. Scoreboard com calendário completo da temporada → última corrida encerrada
            var year = DateTime.UtcNow.Year;
            var boardJson = await client.GetStringAsync(
                $"https://site.api.espn.com/apis/site/v2/sports/racing/f1/scoreboard?dates={year}0101-{year}1231&limit=100");
            var board = JsonNode.Parse(boardJson);

            JsonNode? raceComp = null;
            string raceName = "", circuit = "";
            string raceDate = "";

            // Itera todos os eventos e mantém o último com state=="post" e competition=="Race"
            foreach (var evt in board?["events"]?.AsArray() ?? [])
            {
                var state = evt?["status"]?["type"]?["state"]?.GetValue<string>();
                if (state != "post") continue;

                foreach (var comp in evt?["competitions"]?.AsArray() ?? [])
                {
                    var typeAbbr = comp?["type"]?["abbreviation"]?.GetValue<string>();
                    var compState = comp?["status"]?["type"]?["state"]?.GetValue<string>();
                    if (typeAbbr == "Race" && compState == "post")
                    {
                        raceComp = comp;
                        raceName = evt?["name"]?.GetValue<string>() ?? "";
                        raceDate = evt?["endDate"]?.GetValue<string>() ?? "";
                        circuit = evt?["circuit"]?["fullName"]?.GetValue<string>() ?? "";
                        break;
                    }
                }
                // Não quebra o loop externo — continua para sobrescrever com evento mais recente
            }

            if (raceComp == null)
                return Content("{\"status\":\"no_race\",\"response\":null}", "application/json");

            // 2. Top 3 by finishing position (order field)
            var top3 = (raceComp["competitors"]?.AsArray() ?? [])
                .OrderBy(c => c?["order"]?.GetValue<int>() ?? 99)
                .Take(3)
                .ToList();

            // 3. Championship points from standings
            var standingsJson = await client.GetStringAsync(
                "https://site.api.espn.com/apis/v2/sports/racing/f1/standings");
            var standings = JsonNode.Parse(standingsJson);

            var ptsMap = new Dictionary<string, int>();
            foreach (var entry in standings?["children"]?[0]?["standings"]?["entries"]?.AsArray() ?? [])
            {
                var id = entry?["athlete"]?["id"]?.GetValue<string>() ?? "";
                var pts = (entry?["stats"]?.AsArray() ?? [])
                    .FirstOrDefault(s => s?["name"]?.GetValue<string>() == "championshipPts")
                    ?["value"]?.GetValue<double>() ?? 0;
                if (!string.IsNullOrEmpty(id)) ptsMap[id] = (int)pts;
            }

            // 4. Build response
            var podium = top3.Select(comp =>
            {
                var id = comp?["id"]?.GetValue<string>() ?? "";
                var athlete = comp?["athlete"];
                var name = athlete?["fullName"]?.GetValue<string>() ?? "";
                var (teamName, teamColor) = GetTeamInfo(name);
                ptsMap.TryGetValue(id, out int chPts);

                return (object)new
                {
                    position = comp?["order"]?.GetValue<int>() ?? 0,
                    driverId = id,
                    name,
                    shortName = athlete?["shortName"]?.GetValue<string>() ?? "",
                    teamName,
                    teamColor,
                    flagUrl = athlete?["flag"]?["href"]?.GetValue<string>() ?? "",
                    photoUrl = $"https://a.espncdn.com/i/headshots/f1/players/full/{id}.png",
                    championshipPts = chPts,
                };
            }).ToList();

            var json = JsonSerializer.Serialize(new
            {
                status = "success",
                response = new { raceName, circuit, raceDate, podium }
            });

            cache.Set(cacheKey, json, TimeSpan.FromMinutes(30));
            return Content(json, "application/json");
        }

        private static (string Name, string Color) GetTeamInfo(string driver) => driver switch
        {
            "George Russell" or "Kimi Antonelli"           => ("Mercedes",      "00D2BE"),
            "Lewis Hamilton" or "Charles Leclerc"          => ("Ferrari",        "DC0000"),
            "Lando Norris" or "Oscar Piastri"              => ("McLaren",        "FF8700"),
            "Max Verstappen" or "Liam Lawson"              => ("Red Bull",       "3671C6"),
            "Fernando Alonso" or "Lance Stroll"            => ("Aston Martin",   "006F62"),
            "Pierre Gasly" or "Jack Doohan"                => ("Alpine",         "FE86BC"),
            "Carlos Sainz" or "Alex Albon"                 => ("Williams",       "37BEDD"),
            "Yuki Tsunoda" or "Isack Hadjar"               => ("Racing Bulls",   "6692FF"),
            "Esteban Ocon" or "Oliver Bearman"             => ("Haas",           "B6BABD"),
            "Nico Hülkenberg" or "Gabriel Bortoleto"       => ("Audi",           "FF2D00"),
            _                                               => ("F1",            "E8002D"),
        };
    }
}
