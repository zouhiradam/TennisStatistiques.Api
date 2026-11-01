using NSubstitute;
using TennisStatistiques.Api.Exceptions;
using TennisStatistiques.Api.Repositories;
using TennisStatistiques.Api.Services;
using TennisStatistiques.Api.Tests.Helpers;

namespace TennisStatistiques.Api.Tests.Services
{
    public class StatisticsServiceTests
    {
        private readonly IPlayerRepository _repository = Substitute.For<IPlayerRepository>();
        private readonly StatisticsService _service;

        public StatisticsServiceTests() => _service = new StatisticsService(_repository);

        [Fact]
        public void BestCountry_IsCountryWithHighestWinRatio()
        {
            _repository.GetAll().Returns([
                PlayerBuilder.Create(id: 1, country: "SRB", last: [1, 1, 1, 0]),  // 75 %
            PlayerBuilder.Create(id: 2, country: "FRA", last: [1, 0, 0, 0]),  // 25 %
            PlayerBuilder.Create(id: 3, country: "ESP", last: [1, 1, 0, 0])   // 50 %
            ]);

            var result = _service.GetStatistics();

            Assert.Equal("SRB", result.BestCountry.Code);
            Assert.Equal(0.75, result.BestCountry.WinRatio);
        }

        [Fact]
        public void BestCountry_AggregatesAllMatchesOfACountry()
        {
            _repository.GetAll().Returns([
                PlayerBuilder.Create(id: 1, country: "FRA", last: [1, 1]),        // 2/2
            PlayerBuilder.Create(id: 2, country: "FRA", last: [0, 0, 0, 0]),  // 0/4 → FRA = 2/6
            PlayerBuilder.Create(id: 3, country: "ESP", last: [1, 0])         // 1/2 = 50 %
            ]);

            var result = _service.GetStatistics();

            Assert.Equal("ESP", result.BestCountry.Code);
        }

        [Fact]
        public void AverageBmi_ConvertsGramsAndCentimeters()
        {
            // 80 kg / (2 m)² = 20 ; 90 kg / (1.5 m)² = 40 → moyenne 30
            _repository.GetAll().Returns([
                PlayerBuilder.Create(id: 1, weight: 80000, height: 200),
            PlayerBuilder.Create(id: 2, weight: 90000, height: 150)
            ]);

            var result = _service.GetStatistics();

            Assert.Equal(30, result.AverageBmi);
        }

        [Fact]
        public void GetStatistics_NoPlayers_ThrowsNoPlayersException()
        {
            _repository.GetAll().Returns([]);

            var exception = Assert.Throws<NoPlayersException>(() => _service.GetStatistics());

            Assert.Equal("Aucun joueur disponible pour calculer les statistiques.", exception.Message);
        }

        [Theory]
        [InlineData(new[] { 185, 170, 190 }, 185)]          // impair : valeur centrale
        [InlineData(new[] { 190, 170, 180, 185 }, 182.5)]   // pair : moyenne des deux centrales
        [InlineData(new[] { 175 }, 175)]                    // un seul joueur
        public void MedianHeight_IsCorrect(int[] heights, double expected)
        {
            _repository.GetAll().Returns(
                heights.Select((h, i) => PlayerBuilder.Create(id: i + 1, height: h)).ToList());

            var result = _service.GetStatistics();

            Assert.Equal(expected, result.MedianHeight);
        }
    }
}
