using System.Text.Json.Serialization;

namespace TennisStatistiques.Api.Entities
{
    public class PlayersResponse
    {
        [JsonPropertyName("players")]
        public List<Player> Players { get; set; } = new();
    }
}
