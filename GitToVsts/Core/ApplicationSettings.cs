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
    public string GitPassword
    {
        get => _appSettingByKey.ValueFor("GitPassword");
        set => _appSettingByKey.RunFor("GitPassword", value);
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
    public string VsUser
    {
        get => _appSettingByKey.ValueFor("VsUser");
        set => _appSettingByKey.RunFor("VsUser", value);
    }

    /// <summary>
    /// </summary>
    public string VsPassword
    {
        get => _appSettingByKey.ValueFor("VsPassword");
        set => _appSettingByKey.RunFor("VsPassword", value);
    }

    /// <summary>
    /// </summary>
    public string VsProject
    {
        get => _appSettingByKey.ValueFor("VsProject");
        set => _appSettingByKey.RunFor("VsProject", value);
    }

    /// <summary>
    /// </summary>
    public string VsSource
    {
        get => _appSettingByKey.ValueFor("VsSource");
        set => _appSettingByKey.RunFor("VsSource", value);
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