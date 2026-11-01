using System.Text.Json;
using TennisStatistiques.Api.Entities;

namespace TennisStatistiques.Api.Repositories
{
    public class JsonPlayerRepository: IPlayerRepository
    {
        private readonly List<Player> _players;
        private readonly Lock _lock = new();
        public JsonPlayerRepository()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "headtohead.json");
            var json = File.ReadAllText(path);

            var response = JsonSerializer.Deserialize<PlayersResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _players = response?.Players ?? [];
        }
        public IReadOnlyList<Player> GetAll()
        {
            lock (_lock) return _players.ToList();
        }
        public Player Add(Player player)
        {
            lock (_lock) {
                player.Id = _players.Count == 0 ? 1 : _players.Max(p => p.Id) + 1;
                _players.Add(player);
                return player;
            }
        }
    }
}

