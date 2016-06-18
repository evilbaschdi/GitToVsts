using System;
using System.Diagnostics;

namespace GitToVsts.Internal.Git
{
    public class GetGitProcess : IGitProcess
    {
        private readonly IGitProcessInfo _gitProcessInfo;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public GetGitProcess(IGitProcessInfo gitProcessInfo)
        {
            if (gitProcessInfo == null)
            {
                throw new ArgumentNullException(nameof(gitProcessInfo));
            }
            _gitProcessInfo = gitProcessInfo;
        }

        public void Run(string arguments, string directory)
        {
            var gitInfo = _gitProcessInfo.Value;
            var gitResetHead = new Process();
            gitInfo.Arguments = arguments;
            gitInfo.WorkingDirectory = directory;
            gitResetHead.StartInfo = gitInfo;
            gitResetHead.Start();
            gitResetHead.WaitForExit();
            gitResetHead.Close();
        }
    }
}