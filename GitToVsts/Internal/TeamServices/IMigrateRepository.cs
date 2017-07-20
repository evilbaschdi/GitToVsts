using EvilBaschdi.Core.DotNetExtensions;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Migrates a github repository to visualstudio team services.
    /// </summary>
    public interface IMigrateRepository : IValueFor<GitRepository, Response<string>>
    {
    }
}