using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Interface for requesting DevOps Repositories.
/// </summary>
public interface IRepositories : IValue<DevOpsRepositories>;