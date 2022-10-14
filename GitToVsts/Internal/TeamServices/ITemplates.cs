using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Interface for requesting vsts process templates.
/// </summary>
public interface ITemplates : IValue<VsTsProcessTemplates>
{
}