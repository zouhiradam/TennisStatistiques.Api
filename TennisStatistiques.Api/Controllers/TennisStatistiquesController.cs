using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TennisStatistiques.Api.Dtos;
using TennisStatistiques.Api.Entities;
using TennisStatistiques.Api.Exceptions;
using TennisStatistiques.Api.Services;

namespace TennisStatistiques.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TennisStatistiquesController(IPlayerService playerService, IStatisticsService statisticsService) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Player>> GetAll() =>
        Ok(playerService.GetAllSortedByRank());

        [HttpGet("{id:int}")]
        public ActionResult<Player> GetById(int id)
        {
            var player = playerService.GetById(id);
            return player is null ? NotFound() : Ok(player);
        }
        [HttpGet("statistics")]
        [ProducesResponseType<StatisticsDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
        public ActionResult<StatisticsDto> GetStatistics()=> 
            Ok(statisticsService.GetStatistics());

        [HttpPost]
        [ProducesResponseType<Player>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public ActionResult<Player> Create(CreatePlayerRequest request)
        {
            var player = playerService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = player.Id }, player);
        }
    }
}
