using EvilBaschdi.Core.Settings.ByMachineAndUser;
using JetBrains.Annotations;

namespace GitToVsts.Core;

/// <summary>
///     Wrapper around Default Settings.
/// </summary>
public class ApplicationSettings : IApplicationSettings
{
    private readonly IAppSettingByKey _appSettingByKey;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="appSettingByKey"></param>
    public ApplicationSettings([NotNull] IAppSettingByKey appSettingByKey)
    {
        _appSettingByKey = appSettingByKey ?? throw new ArgumentNullException(nameof(appSettingByKey));
    }

    /// <summary>
    /// </summary>
    public string LoggingPath
    {
        get => _appSettingByKey.ValueFor("LoggingPath");
        set => _appSettingByKey.RunFor("LoggingPath", value);
    }

    /// <summary>
    /// </summary>
    public string GitSourceType
    {
        get => _appSettingByKey.ValueFor("GitSourceType");
        set => _appSettingByKey.RunFor("GitSourceType", value);
    }

    /// <summary>
    /// </summary>
    public string GitUser
    {
        get => _appSettingByKey.ValueFor("GitUser");
        set => _appSettingByKey.RunFor("GitUser", value);
    }

    /// <summary>
    /// </summary>
    public string GitPersonalAccessToken
    {
        get => _appSettingByKey.ValueFor("GitPersonalAccessToken");
        set => _appSettingByKey.RunFor("GitPersonalAccessToken", value);
    }

    /// <summary>
    /// </summary>
    public string GitSource
    {
        get => _appSettingByKey.ValueFor("GitSource");
        set => _appSettingByKey.RunFor("GitSource", value);
    }

    /// <summary>
    /// </summary>
    public string DevOpsUser
    {
        get => _appSettingByKey.ValueFor("DevOpsUser");
        set => _appSettingByKey.RunFor("DevOpsUser", value);
    }

    /// <summary>
    /// </summary>
    public string DevOpsProjectCollection
    {
        get => _appSettingByKey.ValueFor("DevOpsProjectCollection");
        set => _appSettingByKey.RunFor("DevOpsProjectCollection", value);
    }

    /// <summary>
    /// </summary>
    public string DevOpsPersonalAccessToken
    {
        get => _appSettingByKey.ValueFor("DevOpsPersonalAccessToken");
        set => _appSettingByKey.RunFor("DevOpsPersonalAccessToken", value);
    }

    /// <summary>
    /// </summary>
    public string DevOpsProject
    {
        get => _appSettingByKey.ValueFor("DevOpsProject");
        set => _appSettingByKey.RunFor("DevOpsProject", value);
    }

    /// <summary>
    /// </summary>
    public string DevOpsSource
    {
        get => _appSettingByKey.ValueFor("DevOpsSource");
        set => _appSettingByKey.RunFor("DevOpsSource", value);
    }

    /// <summary>
    /// </summary>
    public string TempPath
    {
        get => _appSettingByKey.ValueFor("TempPath");
        set => _appSettingByKey.RunFor("TempPath", value);
    }

    /// <summary>
    /// </summary>
    public bool DeleteTempRepos
    {
        get => Convert.ToBoolean(_appSettingByKey.ValueFor("DeleteTempRepos"));
        set => _appSettingByKey.RunFor("DeleteTempRepos", value.ToString());
    }

    /// <summary>
    /// </summary>
    public string GitBinPath
    {
        get => _appSettingByKey.ValueFor("GitBinPath");
        set => _appSettingByKey.RunFor("GitBinPath", value);
    }
}
