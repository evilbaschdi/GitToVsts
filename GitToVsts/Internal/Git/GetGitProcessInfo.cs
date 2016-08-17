using System;
using System.Diagnostics;
using GitToVsts.Core;

namespace GitToVsts.Internal.Git
{
    public class GetGitProcessInfo : IGitProcessInfo
    {
        private readonly IApplicationSettings _applicationSettings;


        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="applicationSettings" /> is <see langword="null" />.</exception>
        public GetGitProcessInfo(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

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
}