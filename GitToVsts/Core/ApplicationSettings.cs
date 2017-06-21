using System;
using System.IO;
using GitToVsts.Properties;

namespace GitToVsts.Core
{
    /// <summary>
    ///     Wrapper around Default Settings.
    /// </summary>
    public class ApplicationSettings : IApplicationSettings
    {
        /// <summary>
        /// </summary>
        public string LoggingPath
        {
            get => Get("LoggingPath", $@"{Path.GetPathRoot(Environment.SystemDirectory)}G2V");
            set => Set("LoggingPath", value);
        }

        /// <summary>
        /// </summary>
        public string GitSourceType
        {
            get => Get("GitSourceType", "users");
            set => Set("GitSourceType", value);
        }

        /// <summary>
        /// </summary>
        public string GitUser
        {
            get => Get("GitUser", "");
            set => Set("GitUser", value);
        }

        /// <summary>
        /// </summary>
        public string GitPassword
        {
            get => Get("GitPassword", "");
            set => Set("GitPassword", value);
        }

        /// <summary>
        /// </summary>
        public string GitSource
        {
            get => Get("GitSource", "");
            set => Set("GitSource", value);
        }

        /// <summary>
        /// </summary>
        public string VsUser
        {
            get => Get("VsUser", "");
            set => Set("VsUser", value);
        }

        /// <summary>
        /// </summary>
        public string VsPassword
        {
            get => Get("VsPassword", "");
            set => Set("VsPassword", value);
        }

        /// <summary>
        /// </summary>
        public string VsProject
        {
            get => Get("VsProject", "");
            set => Set("VsProject", value);
        }

        /// <summary>
        /// </summary>
        public string VsSource
        {
            get => Get("VsSource", "");
            set => Set("VsSource", value);
        }

        /// <summary>
        /// </summary>
        public string TempPath
        {
            get => Get("TempPath", $@"{Path.GetPathRoot(Environment.SystemDirectory)}G2V");
            set => Set("TempPath", value);
        }

        /// <summary>
        /// </summary>
        public bool DeleteTempRepos
        {
            get => Get<bool>("DeleteTempRepos");
            set => Set("DeleteTempRepos", value);
        }

        /// <summary>
        /// </summary>
        public string GitBinPath
        {
            get => Get("GitBinPath", $@"{Path.GetPathRoot(Environment.SystemDirectory)}Program Files\Git\bin");
            set => Set("GitBinPath", value);
        }


        private T Get<T>(string setting, T fallback = default(T))
        {
            var value = (T) Settings.Default[setting];
            if (fallback != null)
            {
                if (IsValueEmpty(value))
                {
                    return fallback;
                }
            }
            return value;
        }

        private void Set(string setting, object value)
        {
            Settings.Default[setting] = value;
            Settings.Default.Save();
        }

        private bool IsValueEmpty<T>(T value)
        {
            if (value is string)
            {
                if (string.IsNullOrWhiteSpace(value as string))
                {
                    return true;
                }
            }
            else
            {
                if (value.Equals(default(T)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}