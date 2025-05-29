using Microsoft.AspNetCore.Mvc;
using tree_api.API.Contracts.V1;
using tree_api.Domain.Services.TreeService;
using tree_api.Extensions;

namespace tree_api.API.Controllers.TreeController.V1;


[ApiController]
[Route("api/v1/user/tree")]
public class TreeController : ControllerBase
{
    private readonly ITreeService _treeService;

    public TreeController(ITreeService treeService)
    {
        _treeService = treeService;
    }

    /// <summary>
    /// Returns your entire tree. If your tree doesn't exist it will be created automatically.
    /// </summary>
    [HttpPost("get")]
    [ProducesResponseType(typeof(MNode), StatusCodes.Status200OK)]
    public async Task<ActionResult<MNode>> GetTree([FromQuery] string treeName, CancellationToken token)
    {
        var result = await _treeService.GetOrCreateTreeAsync(treeName);

        return Ok(new MNode
        {
            Id = result.Id,
            Name = result.Name,
            Children = result.Nodes.ToArray(n => new MNode
            {
                Id = n.Id,
                Name = n.Name,
                Children = []
            })
        });
    }
}
