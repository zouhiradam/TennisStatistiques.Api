using TennisStatistiques.Api.Entities;

namespace TennisStatistiques.Api.Repositories
{
    public interface IPlayerRepository
    {
        IReadOnlyList<Player> GetAll();
        Player Add(Player player);
    }
}
