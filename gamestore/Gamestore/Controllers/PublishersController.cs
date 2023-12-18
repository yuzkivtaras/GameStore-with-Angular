using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceLayer.Interfaces;
using ServiceLayer.Models.Publisher;

namespace StoreAPI.Controllers;

[ApiController]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;
    private readonly ILogger<PublishersController> _logger;

    public PublishersController(IPublisherService publisherService, ILogger<PublishersController> logger)
    {
        _publisherService = publisherService;
        _logger = logger;
    }

    [HttpPost("publisher/new")]
    public async Task<IActionResult> CreatePublisher([FromBody] PublisherCreateDto publisherDto)
    {
        _logger.LogDebug($"Received publisher model: {JsonConvert.SerializeObject(publisherDto)}");
        _logger.LogDebug($"{nameof(PublishersController)}: Request received to create a new publisher.");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdPlatform = await _publisherService.CreatePublisherAsync(publisherDto);
        return CreatedAtAction(nameof(CreatePublisher), createdPlatform);
    }

    [HttpGet]
    [Route("publishers")]
    public async Task<IActionResult> GetAllPublishers()
    {
        try
        {
            _logger.LogDebug($"{nameof(PublishersController)}: Request received to get all publishers.");

            var publisherModels = await _publisherService.GetAllModelsAsync();

            _logger.LogDebug($"{nameof(PublishersController)}: Successfully retrieved all publishers.");
            return Ok(publisherModels);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(PublishersController)}: Error occurred while getting all publishers. Exception: {ex.Message}");
            return StatusCode(500, "Error while getting all publishers.");
        }
    }

    [HttpGet]
    [Route("publisher/{companyname}")]
    public async Task<IActionResult> GetPublisherDetails(string companyname)
    {
        _logger.LogInformation($"Request with companyName: {companyname}");
        var publisherDetails = await _publisherService.GetPublisherModelDescriptionAsync(companyname);

        return publisherDetails != null ? Ok(publisherDetails) : NotFound();
    }

    [HttpDelete]
    [Route("publisher/remove/{id}")]
    public async Task<IActionResult> DeletePublisherById(string id)
    {
        bool isDeleted = await _publisherService.DeletePublisherAsync(id);
        return !isDeleted ? NotFound() : NoContent();
    }

    [HttpGet]
    [Route("games/publisher/{companyname}")]
    public async Task<ActionResult<IEnumerable<GetGameNameByPublisherDto>>> GetGamesNameByPublisherId(string companyname)
    {
        var gameIdNameDtos = await _publisherService.GetGamesNameByPublisherCompanyName(companyname);

        return gameIdNameDtos == null ? (ActionResult<IEnumerable<GetGameNameByPublisherDto>>)NotFound() : (ActionResult<IEnumerable<GetGameNameByPublisherDto>>)Ok(gameIdNameDtos);
    }

    [HttpPut("publisher/update")]
    public async Task<IActionResult> UpdatePublisher([FromBody] PublisherUpdateDto publisherDto)
    {
        if (publisherDto == null || publisherDto.Publisher == null)
        {
            return BadRequest("Publisher data and ID are required.");
        }

        try
        {
            PublisherResponseForUpdateDto updatedPublisher =
                await _publisherService.UpdatePublisherAsync(publisherDto);
            return CreatedAtAction(nameof(UpdatePublisher), publisherDto);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
}
