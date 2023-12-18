using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Genre;

namespace StoreAPI.Controllers;

[ApiController]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenresController> _logger;

    public GenresController(IGenreService genreService, ILogger<GenresController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    [HttpGet]
    [Route("genres")]
    public async Task<IActionResult> GetAllGenres()
    {
        try
        {
            _logger.LogDebug($"{nameof(GenresController)}: Request received to get all genres.");

            var genreModels = await _genreService.GetAllModelsAsync();

            _logger.LogDebug($"{nameof(GenresController)}: Successfully retrieved all genres.");
            return Ok(genreModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GenresController)}: Error occurred while getting all genres. Exception: {ex.Message}");
            return StatusCode(500, "Error while getting all genres.");
        }
    }

    [HttpPost("genres/new")]
    public async Task<IActionResult> CreateGenre([FromBody] GenreCreateDto genreDto)
    {
        _logger.LogDebug($"Received publisher model: {JsonConvert.SerializeObject(genreDto)}");
        _logger.LogDebug($"{nameof(GenresController)}: Request received to create a new publisher.");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdGenre = await _genreService.CreateGenreAsync(genreDto);
        return CreatedAtAction(nameof(CreateGenre), createdGenre);
    }

    [HttpDelete]
    [Route("genre/remove/{id}")]
    public async Task<IActionResult> DeleteGenreById(string id)
    {
        var genreModel = await _genreService.DeleteGenreModelAsync(id);

        return genreModel == null ? NotFound("Publisher not found") : Ok(genreModel);
    }

    [HttpGet("genre/{id}")]
    public async Task<IActionResult> GetGenreDetails(string id)
    {
        _logger.LogInformation($"Request with id: {id}");
        var genreDetails = await _genreService.GetGenreModelDescriptionAsync(id);

        return genreDetails != null ? Ok(genreDetails) : NotFound();
    }

    [HttpGet]
    [Route("games/genre/{id}")]
    public async Task<ActionResult<IEnumerable<GetGameNameByGenreDto>>> GetGamesNameByGenreId(string id)
    {
        var gameIdNameDtos = await _genreService.GetGamesNameByGenreId(id);

        return gameIdNameDtos == null ? (ActionResult<IEnumerable<GetGameNameByGenreDto>>)NotFound() : (ActionResult<IEnumerable<GetGameNameByGenreDto>>)Ok(gameIdNameDtos);
    }

    [HttpGet]
    [Route("games/parent/{id}")]
    public async Task<ActionResult<IEnumerable<GetGameNameByGenreParentDto>>> GetGamesNameByGenreParentId(string id)
    {
        var gameIdNameDtos = await _genreService.GetGamesNameByParentId(id);

        return gameIdNameDtos == null ? (ActionResult<IEnumerable<GetGameNameByGenreParentDto>>)NotFound() : (ActionResult<IEnumerable<GetGameNameByGenreParentDto>>)Ok(gameIdNameDtos);
    }

    [HttpPut("genre/update")]
    public async Task<IActionResult> UpdateGenre([FromBody] GenreUpdateDto genreDto)
    {
        if (genreDto == null || genreDto.Genre == null)
        {
            return BadRequest("Genre data and ID are required.");
        }

        try
        {
            GenreResponseForUpdateDto updatedGenre =
                await _genreService.UpdateGenreAsync(genreDto);
            return CreatedAtAction(nameof(UpdateGenre), genreDto);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
}
