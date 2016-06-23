using System.IO;

namespace GitToVsts.Core
{
    /// <summary>
    ///     Wrapper arround Default Settings.
    /// </summary>
    public class ApplicationSettings : IApplicationSettings
    {
        /// <summary>
        /// </summary>
        public string LoggingPath
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.LoggingPath)
                        ? Path.GetTempPath()
                        : Properties.Settings.Default.LoggingPath;
            }
            set
            {
                Properties.Settings.Default.LoggingPath = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// </summary>
        public string GitSourceType
        {
            get
            {
                return string.IsNullOrWhiteSpace(Properties.Settings.Default.GitSourceType)
                    ? "users"
                    : Properties.Settings.Default.GitSourceType;
            }
            set
            {
                Properties.Settings.Default.GitSourceType = value;
                Properties.Settings.Default.Save();
            }
        }

        public string GitUser
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.GitUser)
                        ? ""
                        : Properties.Settings.Default.GitUser;
            }
            set
            {
                Properties.Settings.Default.GitUser = value;
                Properties.Settings.Default.Save();
            }
        }

        public string GitPassword
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.GitPassword)
                        ? ""
                        : Properties.Settings.Default.GitPassword;
            }
            set
            {
                Properties.Settings.Default.GitPassword = value;
                Properties.Settings.Default.Save();
            }
        }

        public string GitSource
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.GitSource)
                        ? ""
                        : Properties.Settings.Default.GitSource;
            }
            set
            {
                Properties.Settings.Default.GitSource = value;
                Properties.Settings.Default.Save();
            }
        }

        public string VsUser
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.VsUser)
                        ? ""
                        : Properties.Settings.Default.VsUser;
            }
            set
            {
                Properties.Settings.Default.VsUser = value;
                Properties.Settings.Default.Save();
            }
        }

        public string VsPassword
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.VsPassword)
                        ? ""
                        : Properties.Settings.Default.VsPassword;
            }
            set
            {
                Properties.Settings.Default.VsPassword = value;
                Properties.Settings.Default.Save();
            }
        }

        public string VsProject
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.VsProject)
                        ? ""
                        : Properties.Settings.Default.VsProject;
            }
            set
            {
                Properties.Settings.Default.VsProject = value;
                Properties.Settings.Default.Save();
            }
        }

        public string VsSource
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.VsSource)
                        ? ""
                        : Properties.Settings.Default.VsSource;
            }
            set
            {
                Properties.Settings.Default.VsSource = value;
                Properties.Settings.Default.Save();
            }
        }

        public string TempPath
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.TempPath)
                        ? @"C:\Temp"
                        : Properties.Settings.Default.TempPath;
            }
            set
            {
                Properties.Settings.Default.TempPath = value;
                Properties.Settings.Default.Save();
            }
        }

        public string GitBinPath
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(Properties.Settings.Default.GitBinPath)
                        ? @"C:\Program Files\Git\bin"
                        : Properties.Settings.Default.GitBinPath;
            }
            set
            {
                Properties.Settings.Default.GitBinPath = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}