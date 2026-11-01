namespace TennisStatistiques.Api.Dtos
{
    public record StatisticsDto(CountryRatioDto BestCountry,
    double AverageBmi,
    double MedianHeight);
    public record CountryRatioDto(string Code, double WinRatio);
}
