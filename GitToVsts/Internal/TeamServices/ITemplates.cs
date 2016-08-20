using GitToVsts.Core;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Interface for requesting vsts process templates.
    /// </summary>
    public interface ITemplates : IValue<VsTsProcessTemplates>
    {
    }
}