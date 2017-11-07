using System;
using System.Diagnostics;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Class for git process execution.
    /// </summary>
    public class GetGitProcess : IGitProcess
    {
        private readonly IGitProcessInfo _gitProcessInfo;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="gitProcessInfo" /> is <see langword="null" />.</exception>
        public GetGitProcess(IGitProcessInfo gitProcessInfo)
        {
            _gitProcessInfo = gitProcessInfo ?? throw new ArgumentNullException(nameof(gitProcessInfo));
        }

        /// <summary>
        ///     Calls the local installed git.exe with the given arguments for the given directory.
        /// </summary>
        /// <param name="arguments">arguments to execute git.exe with.</param>
        /// <param name="directory">directory to run git.exe in.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="arguments" /> is <see langword="null" />.
        ///     <paramref name="directory" /> is <see langword="null" />.
        /// </exception>
        public void Run(string arguments, string directory)
        {
            var gitInfo = _gitProcessInfo.Value;
            var gitProcess = new Process();
            gitInfo.Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            gitInfo.WorkingDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            gitProcess.StartInfo = gitInfo;
            gitProcess.Start();
            gitProcess.WaitForExit();
            gitProcess.Close();
        }
    }
}