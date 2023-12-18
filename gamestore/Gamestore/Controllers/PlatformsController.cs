using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceLayer.Interfaces;
using ServiceLayer.Models;
using ServiceLayer.Models.Platform;

namespace StoreAPI.Controllers;

[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformService _platformService;
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(IPlatformService platformService, ILogger<PlatformsController> logger)
    {
        _platformService = platformService;
        _logger = logger;
    }

    [HttpPost("platforms/new")]
    public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreateDto platformDto)
    {
        _logger.LogDebug($"Received publisher model: {JsonConvert.SerializeObject(platformDto)}");
        _logger.LogDebug($"{nameof(PublishersController)}: Request received to create a new publisher.");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdPlatform = await _platformService.CreatePlatformAsync(platformDto);
        return CreatedAtAction(nameof(CreatePlatform), createdPlatform);
    }

    [HttpGet("platform/{id}")]
    public async Task<IActionResult> GetPlatformDetails(string id)
    {
        _logger.LogInformation($"Request with id: {id}");
        var platformDetails = await _platformService.GetPlatformModelDescriptionAsync(id);

        return platformDetails != null ? Ok(platformDetails) : NotFound();
    }

    [HttpGet]
    [Route("platforms")]
    public async Task<IActionResult> GetAllPlatforms()
    {
        try
        {
            _logger.LogDebug($"{nameof(PlatformsController)}: Request received to get all platforms.");

            var platformModels = await _platformService.GetAllModelsAsync();

            _logger.LogDebug($"{nameof(PlatformsController)}: Successfully retrieved all platforms.");
            return Ok(platformModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(PlatformsController)}: Error occurred while getting all publishers. Exception: {ex.Message}");
            return StatusCode(500, "Error while getting all platforms.");
        }
    }

    [HttpDelete]
    [Route("platform/remove/{id}")]
    public async Task<IActionResult> DeletePlatformById(string id)
    {
        bool isDeleted = await _platformService.DeletePlatformAsync(id);
        return !isDeleted ? NotFound() : NoContent();
    }

    [HttpPut("platform/update")]
    public async Task<IActionResult> UpdatePlatform([FromBody] PlatformUpdateDto platformDto)
    {
        if (platformDto == null || platformDto.Platform == null)
        {
            return BadRequest("Platform data and ID are required.");
        }

        try
        {
            PlatformResponseForUpdateDto updatedPlatform =
                await _platformService.UpdatePlatformAsync(platformDto);
            return CreatedAtAction(nameof(UpdatePlatform), platformDto);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }

    [HttpGet("games/platform/{id}")]
    public async Task<ActionResult<IEnumerable<GetGameNameByPlatformDto>>> GetGamesByPlatformId(string id)
    {
        var gameIdNameDtos = await _platformService.GetGamesNameByPlatformId(id);

        return gameIdNameDtos == null ? (ActionResult<IEnumerable<GetGameNameByPlatformDto>>)NotFound() : (ActionResult<IEnumerable<GetGameNameByPlatformDto>>)Ok(gameIdNameDtos);
    }
}
