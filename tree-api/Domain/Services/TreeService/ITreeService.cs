using tree_api.Database.Models;

namespace tree_api.Domain.Services.TreeService;

internal interface ITreeService
{
    Task<Tree> GetOrCreateTreeAsync(string treeName);
}
