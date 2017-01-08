using EvilBaschdi.Core.DotNetExtensions;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Migrates a github repository to vistualstudio team services.
    /// </summary>
    public interface IMigrateRepository : IValueFor<GitRepository, Response<string>>
    {
    }
}