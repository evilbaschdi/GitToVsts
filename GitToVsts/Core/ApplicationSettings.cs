using System.IO;
using EvilBaschdi.CoreExtended.AppHelpers;

namespace GitToVsts.Core;

/// <summary>
///     Wrapper around Default Settings.
/// </summary>
public class ApplicationSettings : IApplicationSettings
{
    private readonly IAppSettingsBase _appSettingsBase;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="appSettingsBase"></param>
    public ApplicationSettings(IAppSettingsBase appSettingsBase)
    {
        _appSettingsBase = appSettingsBase ?? throw new ArgumentNullException(nameof(appSettingsBase));
    }

    /// <summary>
    /// </summary>
    public string LoggingPath
    {
        get => _appSettingsBase.Get("LoggingPath", $@"{Path.GetPathRoot(Environment.SystemDirectory)}G2V");
        set => _appSettingsBase.Set("LoggingPath", value);
    }

    /// <summary>
    /// </summary>
    public string GitSourceType
    {
        get => _appSettingsBase.Get("GitSourceType", "users");
        set => _appSettingsBase.Set("GitSourceType", value);
    }

    /// <summary>
    /// </summary>
    public string GitUser
    {
        get => _appSettingsBase.Get("GitUser", "");
        set => _appSettingsBase.Set("GitUser", value);
    }

    /// <summary>
    /// </summary>
    public string GitPassword
    {
        get => _appSettingsBase.Get("GitPassword", "");
        set => _appSettingsBase.Set("GitPassword", value);
    }

    /// <summary>
    /// </summary>
    public string GitSource
    {
        get => _appSettingsBase.Get("GitSource", "");
        set => _appSettingsBase.Set("GitSource", value);
    }

    /// <summary>
    /// </summary>
    public string VsUser
    {
        get => _appSettingsBase.Get("VsUser", "");
        set => _appSettingsBase.Set("VsUser", value);
    }

    /// <summary>
    /// </summary>
    public string VsPassword
    {
        get => _appSettingsBase.Get("VsPassword", "");
        set => _appSettingsBase.Set("VsPassword", value);
    }

    /// <summary>
    /// </summary>
    public string VsProject
    {
        get => _appSettingsBase.Get("VsProject", "");
        set => _appSettingsBase.Set("VsProject", value);
    }

    /// <summary>
    /// </summary>
    public string VsSource
    {
        get => _appSettingsBase.Get("VsSource", "");
        set => _appSettingsBase.Set("VsSource", value);
    }

    /// <summary>
    /// </summary>
    public string TempPath
    {
        get => _appSettingsBase.Get("TempPath", $@"{Path.GetPathRoot(Environment.SystemDirectory)}G2V");
        set => _appSettingsBase.Set("TempPath", value);
    }

    /// <summary>
    /// </summary>
    public bool DeleteTempRepos
    {
        get => _appSettingsBase.Get<bool>("DeleteTempRepos");
        set => _appSettingsBase.Set("DeleteTempRepos", value);
    }

    /// <summary>
    /// </summary>
    public string GitBinPath
    {
        get => _appSettingsBase.Get("GitBinPath", $@"{Path.GetPathRoot(Environment.SystemDirectory)}Program Files\Git\bin");
        set => _appSettingsBase.Set("GitBinPath", value);
    }
}