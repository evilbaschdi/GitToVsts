using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Interface for requesting visualstudio team services projects.
    /// </summary>
    public interface IProjects : IValue<VsTsProjects>
    {
    }
}