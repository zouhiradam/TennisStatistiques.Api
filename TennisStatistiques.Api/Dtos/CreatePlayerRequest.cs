using System.ComponentModel.DataAnnotations;

namespace TennisStatistiques.Api.Dtos
{
    public record CreatePlayerRequest(
        [Required, StringLength(50)] string Firstname,
        [Required, StringLength(50)] string Lastname,
        [Required, StringLength(10)] string Shortname,
        [Required, RegularExpression("^[MF]$")] string Sex,
        [Required, StringLength(3, MinimumLength = 3)] string CountryCode,
        string? CountryPicture,
        string? Picture,
        [Range(1, int.MaxValue)] int Rank,
        [Range(0, int.MaxValue)] int Points,
        [Range(30000, 200000)] int Weight,   // en grammes
        [Range(100, 250)] int Height,        // en cm
        [Range(10, 70)] int Age);
}
