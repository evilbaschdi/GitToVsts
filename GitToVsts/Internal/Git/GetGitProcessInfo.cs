using System.Diagnostics;
using GitToVsts.Core;

namespace GitToVsts.Internal.Git;

/// <summary>
///     Class for ProcessStartInfo of git.exe.
/// </summary>
public class GetGitProcessInfo : IGitProcessInfo
{
    private readonly IApplicationSettings _applicationSettings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    public GetGitProcessInfo(IApplicationSettings applicationSettings)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
    }

    /// <summary>
    ///     Value of ProcessStartInfo.
    /// </summary>
    public ProcessStartInfo Value
    {
        get
        {
            var gitInfo = new ProcessStartInfo
                          {
                              CreateNoWindow = true,
                              RedirectStandardError = true,
                              RedirectStandardOutput = true,
                              FileName = $@"{_applicationSettings.GitBinPath}\git.exe",
                              UseShellExecute = false
                          };
            return gitInfo;
        }
    }
}