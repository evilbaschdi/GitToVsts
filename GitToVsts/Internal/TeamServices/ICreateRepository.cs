using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates repository through visualstudio.com api.
    /// </summary>
    public interface ICreateRepository : IValue<VsTsRepository>
    {
    }
}