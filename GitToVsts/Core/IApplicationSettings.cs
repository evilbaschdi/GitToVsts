namespace GitToVsts.Core;

/// <summary>
///     Wrapper around Default Settings.
/// </summary>
public interface IApplicationSettings
{
    /// <summary>
    /// </summary>
    bool DeleteTempRepos { get; set; }

    /// <summary>
    /// </summary>
    string GitBinPath { get; set; }

    /// <summary>
    /// </summary>
    string GitPersonalAccessToken { get; set; }

    /// <summary>
    /// </summary>
    string GitSource { get; set; }

    /// <summary>
    /// </summary>
    string GitSourceType { get; set; }

    /// <summary>
    /// </summary>
    string GitUser { get; set; }

    /// <summary>
    /// </summary>
    string LoggingPath { get; set; }

    /// <summary>
    /// </summary>
    string TempPath { get; set; }

    /// <summary>
    /// </summary>
    string DevOpsPersonalAccessToken { get; set; }

    /// <summary>
    /// </summary>
    string DevOpsProjectCollection { get; set; }

    /// <summary>
    /// </summary>
    string DevOpsProject { get; set; }

    /// <summary>
    /// </summary>
    string DevOpsSource { get; set; }

    /// <summary>
    /// </summary>
    string DevOpsUser { get; set; }
}
