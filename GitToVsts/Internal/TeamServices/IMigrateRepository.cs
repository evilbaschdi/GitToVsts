using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    public interface IMigrateRepository
    {
        int For(GitRepository repository);
    }
}