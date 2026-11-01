using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;
using TennisStatistiques.Api.Entities;
using TennisStatistiques.Api.Repositories;

namespace TennisStatistiques.Api.Tests.Integration
{
    public class PlayersEndpointsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        [Fact]
        public async Task GetPlayers_Returns200AndSortedList()
        {
            var players = await _client.GetFromJsonAsync<List<Player>>("/api/TennisStatistiques");

            Assert.NotNull(players);
            Assert.NotEmpty(players);
            Assert.Equal(players.OrderBy(p => p.Data.Rank).Select(p => p.Id),
                         players.Select(p => p.Id));
        }

        [Fact]
        public async Task GetPlayer_UnknownId_Returns404()
        {
            var response = await _client.GetAsync("/api/TennisStatistiques/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostPlayer_InvalidBody_Returns400()
        {
            var response = await _client.PostAsJsonAsync("/api/TennisStatistiques", new { firstname = "" });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetStatistics_Returns200()
        {
            var response = await _client.GetAsync("/api/TennisStatistiques/statistics");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task GetPlayer_ExistingId_Returns200()
        {
            var response = await _client.GetAsync("/api/TennisStatistiques/52");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostPlayer_ValidBody_Returns201WithLocation()
        {
            var request = new
            {
                firstname = "Arthur",
                lastname = "Fils",
                shortname = "A.FIL",
                sex = "M",
                countryCode = "FRA",
                rank = 20,
                points = 1500,
                weight = 79000,
                height = 185,
                age = 22
            };

            var response = await _client.PostAsJsonAsync("/api/TennisStatistiques", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }
        [Fact]
        public async Task GetStatistics_NoPlayers_Returns404WithProblemDetails()
        {
            var emptyRepository = Substitute.For<IPlayerRepository>();
            emptyRepository.GetAll().Returns([]);

            var client = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                    services.Replace(ServiceDescriptor.Singleton(emptyRepository))))
                .CreateClient();

            var response = await client.GetAsync("/api/TennisStatistiques/statistics");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.Equal("Aucun joueur disponible pour calculer les statistiques.", problem?.Detail);
        }
    }
}
