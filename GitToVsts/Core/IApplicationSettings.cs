namespace GitToVsts.Core
{
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
        string GitPassword { get; set; }

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
        string VsPassword { get; set; }

        /// <summary>
        /// </summary>
        string VsProject { get; set; }

        /// <summary>
        /// </summary>
        string VsSource { get; set; }

        /// <summary>
        /// </summary>
        string VsUser { get; set; }
    }
}