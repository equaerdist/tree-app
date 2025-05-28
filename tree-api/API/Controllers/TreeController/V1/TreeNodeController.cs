using Microsoft.AspNetCore.Mvc;
using tree_api.Domain.Services.NodeService;

namespace tree_api.API.Controllers.TreeController.V1;

[ApiController]
[Route("api/v1/user/tree/node")]
public class TreeNodeController : ControllerBase
{
    private readonly INodeService _service;

    internal TreeNodeController(INodeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create a new node in your tree. You must specify a parent node ID that belongs to your tree.
    /// A new node name must be unique across all siblings.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateNode(
        [FromQuery] string treeName,
        [FromQuery] long parentNodeId,
        [FromQuery] string nodeName,
        CancellationToken token)
    {
        await _service.Create(treeName, parentNodeId, nodeName, token);

        return Ok();
    }

    /// <summary>
    /// Delete an existing node in your tree. You must specify a node ID that belongs your tree.
    /// </summary>
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteNode(
        [FromQuery] string treeName,
        [FromQuery] long nodeId,
        CancellationToken token)
    {
        await _service.Delete(treeName, nodeId, token);

        return Ok();
    }

    /// <summary>
    /// Rename an existing node in your tree. You must specify a node ID that belongs your tree.
    /// A new name of the node must be unique across all siblings.
    /// </summary>
    [HttpPost("rename")]
    public async Task<IActionResult> RenameNode(
        [FromQuery] string treeName,
        [FromQuery] long nodeId,
        [FromQuery] string newNodeName,
        CancellationToken token)
    {
        await _service.Rename(treeName, nodeId, newNodeName, token);

        return Ok();
    }
}