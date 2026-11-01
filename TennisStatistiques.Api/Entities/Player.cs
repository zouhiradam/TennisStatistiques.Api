using System.Text.Json.Serialization;

namespace TennisStatistiques.Api.Entities
{
    public class Player
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("firstname")]
        public string Firstname { get; set; } = string.Empty;

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; } = string.Empty;

        [JsonPropertyName("shortname")]
        public string Shortname { get; set; } = string.Empty;

        [JsonPropertyName("sex")]
        public string Sex { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public Country Country { get; set; } = new();

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public PlayerData Data { get; set; } = new();
    }

    public class Country
    {
        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;
    }

    public class PlayerData
    {
        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("points")]
        public int Points { get; set; }

        /// <summary>Poids en grammes (ex : 80000 = 80 kg).</summary>
        [JsonPropertyName("weight")]
        public int Weight { get; set; }

        /// <summary>Taille en centimètres.</summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }

        /// <summary>Résultats des derniers matchs (1 = victoire, 0 = défaite).</summary>
        [JsonPropertyName("last")]
        public List<int> Last { get; set; } = new();
    }
}
