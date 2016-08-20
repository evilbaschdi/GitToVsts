namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Interface for git process class.
    /// </summary>
    public interface IGitProcess
    {
        /// <summary>
        ///     Calls the local installed git.exe with the given arguments for the given directory.
        /// </summary>
        /// <param name="arguments">arguments to execute git.exe with.</param>
        /// <param name="directory">directory to run git.exe in.</param>
        void Run(string arguments, string directory);
    }
}