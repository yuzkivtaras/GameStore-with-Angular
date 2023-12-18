using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Game;

namespace StoreAPI.Controllers;

[ApiController]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(IGameService gameService, ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    // [HttpPut]
    // [Route("game/{key}/buy")]
    // public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
    // {
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    // var createOrder = await _orderService.CreateOrder(orderCreateDto);
    // }
    [HttpPost("games/new")]
    public async Task<IActionResult> CreateGame([FromBody] GameCreateDto gameDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdGame = await _gameService.CreateGameAsync(gameDto);
        return CreatedAtAction(nameof(CreateGame), createdGame);
    }

    [HttpGet("games/{key}")]
    public async Task<IActionResult> GetGameDetails(string key)
    {
        _logger.LogInformation($"Request with id: {key}");
        var gameDetails = await _gameService.GetModelModelDescriptionAsync(key);

        return gameDetails != null ? Ok(gameDetails) : NotFound();
    }

    [HttpGet("search/game/{id}")]
    public async Task<IActionResult> GetGameById(string id)
    {
        var gameDetails = await _gameService.GetModelModelDescriptionByIdAsync(id);

        return gameDetails != null ? Ok(gameDetails) : NotFound();
    }

    [HttpPut("game/update")]
    public async Task<IActionResult> UpdateGame([FromBody] GameUpdateDto gameDto)
    {
        if (gameDto == null || gameDto.Game == null)
        {
            return BadRequest("Game data and ID are required.");
        }

        try
        {
            GameUpdateDto updatedGame =
                await _gameService.UpdateGameAsync(gameDto);
            return CreatedAtAction(nameof(UpdateGame), gameDto);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    [Route("games/{gamealias}/download")]
    public async Task<IActionResult> DownloadGame(string gamealias)
    {
        var gameDownloadDto = new GameDownloadDto { Game = new GameResponseDto { Key = gamealias } };
        var gameResponseForDownloadDto = await _gameService.DownloadGameAsync(gameDownloadDto);
        var content = gameResponseForDownloadDto.FileContent;
        var fileName = gameResponseForDownloadDto.FileName;

        return File(content!, "text/plain", fileName);
    }

    [HttpGet]
    [Route("games")]
    public async Task<IActionResult> GetAllGames()
    {
        try
        {
            var gameModels = await _gameService.GetAllModelsAsync();
            return Ok(gameModels);
        }
        catch (Exception)
        {
            return StatusCode(500, "Error while getting all games.");
        }
    }

    // [HttpGet("count")]
    // public async Task<IActionResult> GetGamesCount()
    // {
    //    try
    //    {
    //        _logger.LogDebug($"{nameof(GamesController)}: Request received to get games count.");

    // var count = await _gameService.GetGamesCountAsync();

    // _logger.LogDebug($"{nameof(GamesController)}: Successfully retrieved games count.");
    //        return Ok(count);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, $"{nameof(GamesController)}: Error occurred while getting games count. Exception: {ex.Message}");
    //        return StatusCode(500, "Error while getting games count.");
    //    }
    // }
    [HttpGet]
    [Route("platforms/game/{id}")]
    public async Task<ActionResult<IEnumerable<GetPlatformNameByGameDto>>> GetPlatformsNameByGameId(string id)
    {
        var platforms = await _gameService.GetPlatformsNameByGame(id);

        return platforms == null ? (ActionResult<IEnumerable<GetPlatformNameByGameDto>>)NotFound() : Ok(platforms);
    }

    [HttpDelete]
    [Route("game/remove/{key}")]
    public async Task<IActionResult> DeleteGameById(string key)
    {
        bool isDeleted = await _gameService.DeleteGameAsync(key);
        return !isDeleted ? NotFound() : NoContent();
    }
}
