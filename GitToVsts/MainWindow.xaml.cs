using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using EvilBaschdi.Core.Application;
using EvilBaschdi.Core.Browsers;
using EvilBaschdi.Core.Wpf;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Internal.Models;
using GitToVsts.Internal.TeamServices;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GitToVsts
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        ///     ObservableColletion to contain GitRepositories.
        /// </summary>
        public ObservableCollection<GitRepositoryObservableCollectionItem> GitRepositoryObservableCollection { get; set; }

        private readonly BackgroundWorker _bw;
        private IGitRepositories _gitRepositories;
        private readonly IMetroStyle _style;
        private readonly IApplicationSettings _applicationSettings;
        private int _overrideProtection;
        private int _executionCount;
        private IProjects _projects;
        private ITemplates _templates;
        private readonly IToast _toast;
        private Configuration _configuration;
        private KeyValuePair<string, string> _result;

        /// <summary>
        ///     MainWindow
        /// </summary>
        public MainWindow()
        {
            ISettings coreSettings = new CoreSettings();
            _applicationSettings = new ApplicationSettings();
            InitializeComponent();
            _bw = new BackgroundWorker();
            _style = new MetroStyle(this, Accent, ThemeSwitch, coreSettings);
            _style.Load(true);
            _toast = new Toast(Title, "baschdi.png");
            var linkerTime = Assembly.GetExecutingAssembly().GetLinkerTime();
            LinkerTime.Content = linkerTime.ToString(CultureInfo.InvariantCulture);
            Load();
        }

        #region GitTab

        //todo: auslagern
        private void LoadGitRepositoryList()
        {
            GitRepositoryObservableCollection = GetGitRepositoryObservableCollection();
            GitRepositoryObservableCollectionBox.ItemsSource = GitRepositoryObservableCollection;
        }

        private ObservableCollection<GitRepositoryObservableCollectionItem> GetGitRepositoryObservableCollection()
        {
            var collection = new ObservableCollection<GitRepositoryObservableCollectionItem>();

            int i = 1;
            foreach (var repository in _gitRepositories.Value)
            {
                collection.Add(new GitRepositoryObservableCollectionItem
                               {
                                   Displayname = $"{i++}_{repository.Name}",
                                   Repository = repository
                               });
            }

            return collection;
        }

        private void GitLoginOnClick(object sender, RoutedEventArgs e)
        {
            _applicationSettings.GitUser = GitUsername.Text;
            _applicationSettings.GitPassword = GitPassword.Password;

            var getGitUser = new GetGitUser(_applicationSettings);
            if (getGitUser.Value != null)
            {
                _applicationSettings.GitSource = GitSource.Text;
                _gitRepositories = new GetGitRepositories(_applicationSettings);
                LoadGitRepositoryList();
                var convertGitAvatar = new ConvertGitAvatart();
                GitAvatar.Source = convertGitAvatar.For(getGitUser.Value);
                GitAvatar.Visibility = Visibility.Visible;
                GitLogin.Visibility = Visibility.Hidden;

                ShowMessage("Successfull", $"'{getGitUser.Value.Login}' was successfully authenticated {Environment.NewLine}Please switch to 'Repositories'");
                RepoTab.IsEnabled = true;
            }
            else
            {
                _applicationSettings.GitUser = "";
                _applicationSettings.GitPassword = "";
            }
        }


        private void SourceType(object sender, EventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            var toggleSwitch = (ToggleSwitch) sender;
            _applicationSettings.GitSourceType = toggleSwitch.IsChecked.HasValue && toggleSwitch.IsChecked.Value ? "orgs" : "users";
        }

        private void ValidateGitTextBoxesOnTextChanged(object sender, EventArgs e)
        {
            GitLogin.IsEnabled = !string.IsNullOrWhiteSpace(GitUsername.Text) &&
                                 !string.IsNullOrWhiteSpace(GitPassword.Password) &&
                                 !string.IsNullOrWhiteSpace(GitSource.Text);
        }

        #endregion GitTab

        #region RepoTab

        private void MigrateToVsTsOnClick(object sender, RoutedEventArgs e)
        {
            SetRepoLabelContent();
        }

        private void MigrateAllOnClick(object sender, RoutedEventArgs e)
        {
            foreach (var gitRepositoryObservableCollectionItem in GitRepositoryObservableCollection)
            {
                gitRepositoryObservableCollectionItem.MigrateToVsTs = true;
            }
            GitRepositoryObservableCollectionBox.ItemsSource = null;
            GitRepositoryObservableCollectionBox.ItemsSource = GitRepositoryObservableCollection;

            SetRepoLabelContent();
        }

        private void SetRepoLabelContent()
        {
            var checkedItemsCount = GitRepositoryObservableCollection.Count(attribute => attribute.MigrateToVsTs);
            RepoLabel.Content = $"Repositories chosen to migrate: {checkedItemsCount}";
            VsTab.IsEnabled = checkedItemsCount != 0;
        }

        #endregion RepoTab

        #region VsTsTab

        private void VsLoginOnClick(object sender, RoutedEventArgs e)
        {
            _applicationSettings.VsUser = VsUsername.Text;
            _applicationSettings.VsPassword = VsPassword.Password;
            _applicationSettings.VsSource = VsSource.Text;
            _applicationSettings.VsProject = VsProjects.Text;

            VsProjects.IsEnabled = true;
            VsTemplates.IsEnabled = true;

            _projects = new GetProjects(_applicationSettings);
            foreach (var project in _projects.Value.Value)
            {
                VsProjects.Items.Add(project.Name);
            }

            _templates = new GetTemplates(_applicationSettings);
            foreach (var template in _templates.Value.Value)
            {
                VsTemplates.Items.Add(template.Name);
            }
            VsLogin.IsEnabled = false;
        }

        private void VsTemplatesOnDropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(VsTemplates.Text) && !string.IsNullOrWhiteSpace(VsTemplates.Text))
            {
                RunTab.IsEnabled = true;
            }
        }

        private void VsProjectsOnDropDownClosed(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(VsTemplates.Text) && !string.IsNullOrWhiteSpace(VsTemplates.Text))
            {
                RunTab.IsEnabled = true;
            }
        }

        private void ValidateVsTextBoxesOnTextChanged(object sender, RoutedEventArgs e)
        {
            VsLogin.IsEnabled = !string.IsNullOrWhiteSpace(VsUsername.Text) &&
                                !string.IsNullOrWhiteSpace(VsPassword.Password) &&
                                !string.IsNullOrWhiteSpace(VsSource.Text);
        }

        #endregion VsTsTab

        #region RunTab

        private void RunMigrationOnClick(object sender, RoutedEventArgs e)
        {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            Run();
        }

        private void Run()
        {
            _executionCount++;
            var configuration = new Configuration
                                {
                                    VsTemplate = VsTemplates.Text,
                                    VsProject = VsProjects.Text
                                };
            Cursor = Cursors.Wait;
            _configuration = configuration;
            if (_executionCount == 1)
            {
                _bw.DoWork += (o, args) => RunRepositoryMigration();
                _bw.WorkerReportsProgress = true;
                _bw.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            }
            _bw.RunWorkerAsync();
        }

        private void RunRepositoryMigration()
        {
            var repoPaths = new ConcurrentBag<string>();
            var configuration = _configuration;
            var checkedItems = GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToVsTs);
            var gitCommands = new GitCommands();
            var migrate = new MigrateRepository(_applicationSettings, _templates, _projects, gitCommands, configuration.VsTemplate, configuration.VsProject);

            var repositoriesToMigrate = checkedItems as IList<GitRepositoryObservableCollectionItem> ?? checkedItems.ToList();
            Parallel.ForEach(repositoriesToMigrate, checkedItem =>
                                                    {
                                                        var response = migrate.For(checkedItem.Repository);
                                                        if (response.Code != 200)
                                                        {
                                                            //todo errorhandling
                                                        }
                                                        else
                                                        {
                                                            repoPaths.Add(response.Value);
                                                        }
                                                    });

            if (_applicationSettings.DeleteTempRepos)
            {
                foreach (var repoPath in repoPaths)
                {
                    if (Directory.Exists(repoPath))
                    {
                        try
                        {
                            Directory.Delete(repoPath);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            _result = new KeyValuePair<string, string>("Finished", $"All {repositoriesToMigrate.Count} repositories were migrated.");
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ShowMessage(_result.Key, _result.Value);
            _toast.Show(_result.Key, _result.Value);

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            Cursor = Cursors.Arrow;
        }

        #endregion RunTab

        #region Window Methods

        private void Load()
        {
            GitUsername.Text = _applicationSettings.GitUser;
            GitPassword.Password = _applicationSettings.GitPassword;
            GitSource.Text = _applicationSettings.GitSource;
            VsUsername.Text = _applicationSettings.VsUser;
            VsPassword.Password = _applicationSettings.VsPassword;
            VsSource.Text = _applicationSettings.VsSource;
            VsProjects.Text = _applicationSettings.VsProject;
            LoggingPath.Text = _applicationSettings.LoggingPath;
            TempPath.Text = _applicationSettings.TempPath;
            GitBinPath.Text = _applicationSettings.GitBinPath;
            CleanUpSwitch.IsChecked = _applicationSettings.DeleteTempRepos;

            switch (_applicationSettings.GitSourceType)
            {
                case "users":
                    SourceSwitch.IsChecked = false;
                    break;

                case "orgs":
                    SourceSwitch.IsChecked = true;
                    break;
            }

            _overrideProtection = 1;
        }

        private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
        {
            var browser = new ExplorerFolderBrowser
                          {
                              SelectedPath = _applicationSettings.LoggingPath
                          };
            browser.ShowDialog();
            _applicationSettings.LoggingPath = browser.SelectedPath;
            LoggingPath.Text = _applicationSettings.LoggingPath;
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(LoggingPath.Text))
            {
                _applicationSettings.LoggingPath = LoggingPath.Text;
            }
        }

        private void BrowseTempPathClick(object sender, RoutedEventArgs e)
        {
            var browser = new ExplorerFolderBrowser
                          {
                              SelectedPath = _applicationSettings.TempPath
                          };
            browser.ShowDialog();
            _applicationSettings.TempPath = browser.SelectedPath;
            TempPath.Text = _applicationSettings.TempPath;
        }

        private void TempPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(TempPath.Text))
            {
                _applicationSettings.TempPath = TempPath.Text;
            }
        }

        private void CleanUp(object sender, EventArgs e)
        {
            var toggleSwitch = (ToggleSwitch) sender;
            _applicationSettings.DeleteTempRepos = toggleSwitch.IsChecked.HasValue && toggleSwitch.IsChecked.Value;
        }

        private void BrowseGitPathClick(object sender, RoutedEventArgs e)
        {
            var browser = new ExplorerFolderBrowser
                          {
                              SelectedPath = _applicationSettings.GitBinPath
                          };
            browser.ShowDialog();
            if (File.Exists($@"{browser.SelectedPath}\git.exe"))
            {
                _applicationSettings.GitBinPath = browser.SelectedPath;
                GitBinPath.Text = _applicationSettings.GitBinPath;
            }
            else
            {
                ShowMessage("Path Error", "Path does not contain a 'git.exe'");
            }
        }

        private void GitPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(GitBinPath.Text) && File.Exists($@"{GitBinPath.Text}\git.exe"))
            {
                _applicationSettings.GitBinPath = GitBinPath.Text;
            }
            else
            {
                ShowMessage("Path Error", "Path does not contain a 'git.exe'");
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        public async void ShowMessage(string title, string message)
        {
            var options = new MetroDialogSettings
                          {
                              ColorScheme = MetroDialogColorScheme.Theme
                          };

            MetroDialogOptions = options;
            await this.ShowMessageAsync(title, message);
        }

        #endregion Window Methods

        #region Flyout

        private void ToggleSettingsFlyoutClick(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void ToggleFlyout(int index, bool stayOpen = false)
        {
            var activeFlyout = (Flyout) Flyouts.Items[index];
            if (activeFlyout == null)
            {
                return;
            }

            foreach (
                var nonactiveFlyout in
                Flyouts.Items.Cast<Flyout>()
                       .Where(nonactiveFlyout => nonactiveFlyout.IsOpen && nonactiveFlyout.Name != activeFlyout.Name))
            {
                nonactiveFlyout.IsOpen = false;
            }

            activeFlyout.IsOpen = activeFlyout.IsOpen && stayOpen || !activeFlyout.IsOpen;
        }

        #endregion Flyout

        #region MetroStyle

        private void SaveStyleClick(object sender, RoutedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SaveStyle();
        }

        private void Theme(object sender, EventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            var routedEventArgs = e as RoutedEventArgs;
            if (routedEventArgs != null)
            {
                _style.SetTheme(sender, routedEventArgs);
            }
            else
            {
                _style.SetTheme(sender);
            }
        }

        private void AccentOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SetAccent(sender, e);
        }

        #endregion MetroStyle
    }
}