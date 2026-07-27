namespace GitToVsts.Core.Internal.TeamServices;

/// <summary>
///     Creates project through visualstudio.com API.
/// </summary>
public interface ICreateProject : IValue<DevOpsCreateResponse>;