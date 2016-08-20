using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Migrates a github repository to vistualstudio team services.
    /// </summary>
    public interface IMigrateRepository
    {
        /// <summary>
        ///     Contains the response code of repository migration.
        /// </summary>
        /// <param name="repository">GitRepository to migrate.</param>
        /// <returns></returns>
        int For(GitRepository repository);
    }
}