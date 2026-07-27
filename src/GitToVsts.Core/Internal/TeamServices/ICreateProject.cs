using EvilBaschdi.Core;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Creates project through visualstudio.com API.
/// </summary>
public interface ICreateProject : IValue<DevOpsCreateResponse>;