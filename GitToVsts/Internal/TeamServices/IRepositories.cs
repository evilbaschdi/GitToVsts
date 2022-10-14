using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Interface for requesting Vsts Repositories.
/// </summary>
public interface IRepositories : IValue<VsTsRepositories>
{
}