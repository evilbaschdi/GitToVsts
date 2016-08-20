using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates project through visualstudio.com api.
    /// </summary>
    public interface ICreateProject : IValue<VsTsCreateResponse>
    {
    }
}