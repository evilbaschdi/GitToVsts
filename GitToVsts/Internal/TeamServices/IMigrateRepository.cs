using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Migrates a github repository to vistualstudio team services.
    /// </summary>
    public interface IMigrateRepository
    {
        /// <summary>
        ///     Contains the response of repository migration.
        /// </summary>
        /// <param name="repository">GitRepository to migrate.</param>
        /// <returns></returns>
        Response<string> For(GitRepository repository);
    }
}