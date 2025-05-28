using Microsoft.AspNetCore.Mvc;
using tree_api.API.Contracts.V1;
using tree_api.Domain.Services.JournalService;
using tree_api.Extensions;

namespace tree_api.API.Controllers.TreeController.V1;

[ApiController]
[Route("api/v1/user/journal")]
public class JournalController : ControllerBase
{
    private readonly IJournalService _service;

    internal JournalController(IJournalService service)
    {
        _service = service;
    }

    /// <summary>
    /// Provides the pagination API. Skip means the number of items should be skipped by server.
    /// Take means the maximum number items should be returned by server.
    /// All fields of the filter are optional.
    /// </summary>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The maximum number of items to return.</param>
    /// <param name="filter">Optional filter criteria.</param>
    /// <returns>Paginated list of journal entries.</returns>
    [HttpPost("getRange")]
    [ProducesResponseType(typeof(MRange_MJournalInfo), StatusCodes.Status200OK)]
    public async Task<ActionResult<MRange_MJournalInfo>> GetJournalRange(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromBody] VJournalFilter? filter,
        CancellationToken token)
    {
        var result = await _service.GetJournals(skip, take, filter, token);

        return Ok(new MRange_MJournalInfo
        {
            Skip = skip,
            Count = result.Count,
            Items = result.Items.ToArray(x => new MJournalInfo
            {
                Id = x.Id,
                EventId = long.TryParse(x.EventId, out var eid) ? eid : 0,
                CreatedAt = x.CreatedAt.DateTime
            })
        });
    }

    /// <summary>
    /// Returns the information about a particular event by ID.
    /// </summary>
    [HttpPost("getSingle")]
    [ProducesResponseType(typeof(MJournal), StatusCodes.Status200OK)]
    public async Task<ActionResult<MJournal>> GetSingle([FromQuery] long id, CancellationToken token)
    {
        var result = await _service.GetSingle(id, token);

        if (result == null)
            return NotFound();

        return new MJournal
        {
            Id = result.Id,
            EventId = long.TryParse(result.EventId, out var eid) ? eid : 0,
            CreatedAt = result.CreatedAt,
            Text = result.StackTrace
        };
    }
}
