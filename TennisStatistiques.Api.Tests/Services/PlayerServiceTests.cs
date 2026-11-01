using NSubstitute;
using TennisStatistiques.Api.Dtos;
using TennisStatistiques.Api.Entities;
using TennisStatistiques.Api.Repositories;
using TennisStatistiques.Api.Services;
using TennisStatistiques.Api.Tests.Helpers;

namespace TennisStatistiques.Api.Tests.Services
{
    public class PlayerServiceTests
    {
        private readonly IPlayerRepository _repository = Substitute.For<IPlayerRepository>();
        private readonly PlayerService _service;
        public PlayerServiceTests() => _service = new PlayerService(_repository);
        [Fact]
        public void GetAllSortedByRank_ReturnsPlayersFromBestToWorst()
        {
            _repository.GetAll().Returns([
                PlayerBuilder.Create(id:1,rank:3),
            PlayerBuilder.Create(id:2,rank:1),
            PlayerBuilder.Create(id:3,rank:2)
            ]);
            var result = _service.GetAllSortedByRank().Select(p => p.Data.Rank);
            Assert.Equal([1,2,3],result);
        }

        [Fact]
        public void GetById_ExistingId_ReturnsPlayer()
        {
            _repository.GetAll().Returns([PlayerBuilder.Create(id:52)]);
            var result = _service.GetById(52);
            Assert.NotNull(result);
            Assert.Equal(52, result.Id);
        }

        [Fact]
        public void GetById_UnknownId_ReturnsNull()
        {
            _repository.GetAll().Returns([PlayerBuilder.Create(id: 52)]);

            Assert.Null(_service.GetById(999));
        }

        [Fact]
        public void Create_MapsRequestAndCallsRepository()
        {
            var request = new CreatePlayerRequest(
                "Arthur", "Fils", "A.FIL", "M", "fra", null, null,
                Rank: 20, Points: 1500, Weight: 79000, Height: 185, Age: 22);

            _repository.Add(Arg.Any<Player>()).Returns(ci => ci.Arg<Player>());

            var result = _service.Create(request);

            Assert.Equal("Arthur", result.Firstname);
            Assert.Equal("FRA", result.Country.Code);   // mis en majuscules
            Assert.Empty(result.Data.Last);
            _repository.Received(1).Add(Arg.Any<Player>());
        }

    }
}
