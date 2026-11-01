using TennisStatistiques.Api.Dtos;
using TennisStatistiques.Api.Entities;
using TennisStatistiques.Api.Repositories;

namespace TennisStatistiques.Api.Services
{
    public class PlayerService(IPlayerRepository repository) : IPlayerService
    {
        public Player Create(CreatePlayerRequest request)
        {
            var player = new Player
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Shortname = request.Shortname,
                Sex = request.Sex,
                Picture = request.Picture ?? string.Empty,
                Country = new Country
                {
                    Code = request.CountryCode.ToUpperInvariant(),
                    Picture = request.CountryPicture ?? string.Empty
                },
                Data = new PlayerData
                {
                    Rank = request.Rank,
                    Points = request.Points,
                    Weight = request.Weight,
                    Height = request.Height,
                    Age = request.Age,
                    Last = []   // nouveau joueur : aucun match joué
                }
            };

            return repository.Add(player);
        }

        public IEnumerable<Player> GetAllSortedByRank() =>
        repository.GetAll().OrderBy(p => p.Data.Rank);

        public Player? GetById(int id) =>
            repository.GetAll().FirstOrDefault(p => p.Id == id);
    }
}
