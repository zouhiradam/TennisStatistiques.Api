using TennisStatistiques.Api.Dtos;

namespace TennisStatistiques.Api.Services
{
    public interface IStatisticsService
    {
        StatisticsDto GetStatistics();
    }
}
