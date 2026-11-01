using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using TennisStatistiques.Api.Controllers;
using TennisStatistiques.Api.Dtos;
using TennisStatistiques.Api.Exceptions;
using TennisStatistiques.Api.Services;

namespace TennisStatistiques.Api.Tests.Controllers
{
    public class TennisStatistiquesControllerTests
    {
        private readonly IPlayerService _playerService = Substitute.For<IPlayerService>();
        private readonly IStatisticsService _statisticsService = Substitute.For<IStatisticsService>();
        private readonly TennisStatistiquesController _controller;

        public TennisStatistiquesControllerTests() =>
      _controller = new TennisStatistiquesController(_playerService, _statisticsService);

        [Fact]
        public void GetStatistics_NoPlayers_LetsExceptionPropagate()
        {
            _statisticsService.GetStatistics().Throws(new NoPlayersException());

            Assert.Throws<NoPlayersException>(() => _controller.GetStatistics());
        }

        [Fact]
        public void GetStatistics_WithPlayers_ReturnsOkWithStatistics()
        {
            var expected = new StatisticsDto(new CountryRatioDto("SRB", 1.0), 23.36, 185);
            _statisticsService.GetStatistics().Returns(expected);

            var response = _controller.GetStatistics();

            var ok = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(expected, ok.Value);
        }
    }
}
