using EvilBaschdi.Core.DotNetExtensions;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Interface for requesting visualstudio teamservices projects.
    /// </summary>
    public interface IProjects : IValue<VsTsProjects>
    {
    }
}