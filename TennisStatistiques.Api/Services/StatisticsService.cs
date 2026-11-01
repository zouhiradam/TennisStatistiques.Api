using TennisStatistiques.Api.Dtos;
using TennisStatistiques.Api.Entities;
using TennisStatistiques.Api.Exceptions;
using TennisStatistiques.Api.Repositories;

namespace TennisStatistiques.Api.Services
{
    public class StatisticsService(IPlayerRepository repository) : IStatisticsService
    {
        public StatisticsDto GetStatistics()
        {
            var players = repository.GetAll();
            if (players.Count == 0)
                throw new NoPlayersException();
            return new StatisticsDto(
                GetBestCountry(players),
                GetAverageBmi(players),
                GetMedianHeight(players));
        }
        // Ratio = total des victoires / total des matchs, cumulés par pays
        private static CountryRatioDto GetBestCountry(IReadOnlyList<Player> players) =>
            players
                .GroupBy(p => p.Country.Code)
                .Select(g =>
                {
                    var results = g.SelectMany(p => p.Data.Last).ToList();
                    var ratio = results.Count == 0 ? 0 : (double)results.Sum() / results.Count;
                    return new CountryRatioDto(g.Key, Math.Round(ratio, 2));
                })
                .OrderByDescending(c => c.WinRatio)
                .First();

        // IMC = poids (kg) / taille (m)² ; poids en grammes, taille en cm dans le JSON
        private static double GetAverageBmi(IReadOnlyList<Player> players) =>
            Math.Round(players.Average(p =>
            {
                var weightKg = p.Data.Weight / 1000.0;
                var heightM = p.Data.Height / 100.0;
                return weightKg / (heightM * heightM);
            }), 2);

        private static double GetMedianHeight(IReadOnlyList<Player> players)
        {
            var heights = players.Select(p => p.Data.Height).Order().ToList();
            var middle = heights.Count / 2;

            return heights.Count % 2 == 0
                ? (heights[middle - 1] + heights[middle]) / 2.0
                : heights[middle];
        }
    }
}
