using TennisStatistiques.Api.Dtos;
using TennisStatistiques.Api.Entities;

namespace TennisStatistiques.Api.Services
{
    public interface IPlayerService
    {
        IEnumerable<Player> GetAllSortedByRank();
        Player? GetById(int id);
        Player Create(CreatePlayerRequest request);
    }
}
