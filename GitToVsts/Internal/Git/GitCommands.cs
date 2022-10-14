namespace GitToVsts.Internal.Git;

/// <summary>
///     Git Commands.
/// </summary>
public class GitCommands : IGitCommands
{
    /// <summary>
    /// </summary>
    public string Clone { get; } = "clone --mirror";

    /// <summary>
    /// </summary>
    public string Config { get; } = "config --local --bool core.bare false";

    /// <summary>
    /// </summary>
    public string RemoteAdd { get; } = "remote add vsts";

    /// <summary>
    /// </summary>
    public string PushAll { get; } = "push --all vsts";

    /// <summary>
    /// </summary>
    public string PushTags { get; } = "push --tags vsts";
}