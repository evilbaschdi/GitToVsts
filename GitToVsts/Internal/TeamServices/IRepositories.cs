using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Interface for requesting Vsts Repositories.
    /// </summary>
    public interface IRepositories : IValue<VsTsRepositories>
    {
    }
}