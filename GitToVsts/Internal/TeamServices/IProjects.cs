using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Interface for requesting visualstudio teamservices projects.
    /// </summary>
    public interface IProjects : IValue<VsTsProjects>
    {
    }
}