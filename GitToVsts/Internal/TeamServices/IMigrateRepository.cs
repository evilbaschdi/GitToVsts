using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Migrates a github repository to visualstudio team services.
/// </summary>
public interface IMigrateRepository : IValueFor<GitRepository, Response<string>>;