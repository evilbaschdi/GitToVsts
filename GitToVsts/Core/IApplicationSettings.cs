namespace GitToVsts.Core
{
    /// <summary>
    ///     Wrapper arround Default Settings.
    /// </summary>
    public interface IApplicationSettings
    {
        /// <summary>
        /// </summary>
        string LoggingPath { get; set; }

        string GitSourceType { get; set; }

        string GitUser { get; set; }
        string GitPassword { get; set; }
        string GitSource { get; set; }
        string VsUser { get; set; }
        string VsPassword { get; set; }
        bool VsNewProject { get; set; }
        string VsProject { get; set; }
        string VsSource { get; set; }
        string TempPath { get; set; }
        string GitBinPath { get; set; }
    }
}