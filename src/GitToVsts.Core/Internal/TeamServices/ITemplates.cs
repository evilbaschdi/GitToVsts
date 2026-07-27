using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Interface for requesting devops process templates.
/// </summary>
public interface ITemplates : IValue<DevOpsProcessTemplates>;