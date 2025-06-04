using tree_api.Database.Models;

namespace tree_api.Domain.Services.TreeService;

public interface ITreeService
{
    Task<Tree> GetOrCreateTreeAsync(string treeName, CancellationToken token);
}
