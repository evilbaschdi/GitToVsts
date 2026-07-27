namespace GitToVsts.Core.Internal.TeamServices;

/// <summary>
///     Migrates a github repository to visualstudio team services.
/// </summary>
public interface IMigrateRepository : IValueFor<GitRepository, Response<string>>;