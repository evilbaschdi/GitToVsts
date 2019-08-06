using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using EvilBaschdi.CoreExtended;
using EvilBaschdi.CoreExtended.AppHelpers;
using EvilBaschdi.CoreExtended.Browsers;
using EvilBaschdi.CoreExtended.Metro;
using EvilBaschdi.CoreExtended.Mvvm;
using EvilBaschdi.CoreExtended.Mvvm.View;
using EvilBaschdi.CoreExtended.Mvvm.ViewModel;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Internal.TeamServices;
using GitToVsts.Model;
using GitToVsts.Properties;
using JetBrains.Annotations;
using MahApps.Metro;
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
        private readonly Brush _accentColorBrush;
        private readonly IApplicationSettings _applicationSettings;
        private readonly IDialogService _dialogService;
        private readonly ObservableCollection<GitRepositoryObservableCollectionItem> _migrationFailedRepos = new ObservableCollection<GitRepositoryObservableCollectionItem>();
        private readonly ObservableCollection<GitRepositoryObservableCollectionItem> _migrationSuccessRepos = new ObservableCollection<GitRepositoryObservableCollectionItem>();
        private readonly IThemeManagerHelper _themeManagerHelper;
        private IMigrationConfiguration _configuration;
        private ProgressDialogController _controller;

        //private read-only BackgroundWorker _bw;
        private IGitRepositories _gitRepositories;

        private int _overrideProtection;
        private IProjects _projects;
        private KeyValuePair<string, string> _result;
        private ITemplates _templates;

        /// <summary>
        ///     MainWindow
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            IAppSettingsBase appSettingsBase = new AppSettingsBase(Settings.Default);
            _applicationSettings = new ApplicationSettings(appSettingsBase);
            _themeManagerHelper = new ThemeManagerHelper();

            var applicationStyle = new ApplicationStyle(_themeManagerHelper);
            applicationStyle.Load(true);
            _accentColorBrush = (Brush)ThemeManager.GetResourceFromAppStyle(this, "MahApps.Brushes.AccentBase");

            _dialogService = new DialogService(this);
            Load();
        }

        /// <summary>
        ///     ObservableCollection to contain GitRepositories.
        /// </summary>
        private ObservableCollection<GitRepositoryObservableCollectionItem> GitRepositoryObservableCollection { get; set; }


        #region GitTab

        private void LoadGitRepositoryList()
        {
            GitRepositoryObservableCollection = GetGitRepositoryObservableCollection();
            GitRepositoryObservableCollectionBox.ItemsSource = GitRepositoryObservableCollection;
            GitRepositoryMigrationFailedObservableCollectionBox.ItemsSource = _migrationFailedRepos;
            GitRepositoryMigrationSuccessObservableCollectionBox.ItemsSource = _migrationSuccessRepos;
        }

        private ObservableCollection<GitRepositoryObservableCollectionItem> GetGitRepositoryObservableCollection()
        {
            var collection = new ObservableCollection<GitRepositoryObservableCollectionItem>();

            var i = 1;
            foreach (var repository in _gitRepositories.Value)
            {
                collection.Add(new GitRepositoryObservableCollectionItem
                               {
                                   DisplayName = $"{i++}_{repository.Name}",
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
                var convertGitAvatar = new ConvertGitAvatar();
                GitAvatar.Source = convertGitAvatar.ValueFor(getGitUser.Value);
                GitAvatar.Visibility = Visibility.Visible;
                GitLogin.Visibility = Visibility.Hidden;

                _dialogService.ShowMessage("Successful", $"'{getGitUser.Value.Login}' was successfully authenticated {Environment.NewLine}Please switch to 'Repositories'");
                RepoTab.IsEnabled = true;
                RepoTab.Background = _accentColorBrush;
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
            // ReSharper disable once StringLiteralTypo
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
            VsTab.Background = VsTab.IsEnabled ? _accentColorBrush : GitTab.Background;
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
            MigrationFailedTab.IsEnabled = true;
            SuccessfulTab.IsEnabled = true;

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
            if (string.IsNullOrWhiteSpace(VsTemplates.Text) || string.IsNullOrWhiteSpace(VsTemplates.Text))
            {
                return;
            }

            RunTab.IsEnabled = true;
            RunTab.Background = _accentColorBrush;
        }

        private void VsProjectsOnDropDownClosed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(VsTemplates.Text) || string.IsNullOrWhiteSpace(VsTemplates.Text))
            {
                return;
            }

            RunTab.IsEnabled = true;
            RunTab.Background = _accentColorBrush;
        }

        private void ValidateVsTextBoxesOnTextChanged(object sender, RoutedEventArgs e)
        {
            VsLogin.IsEnabled = !string.IsNullOrWhiteSpace(VsPassword.Password) &&
                                !string.IsNullOrWhiteSpace(VsSource.Text);
        }

        #endregion VsTsTab

        #region RunTab

        private async void RunMigrationOnClickAsync(object sender, RoutedEventArgs e)
        {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            // ReSharper disable once AsyncConverter.AsyncAwaitMayBeElidedHighlighting
            await RunAsync().ConfigureAwait(true);
        }

        private async Task RunAsync()
        {
            IMigrationConfiguration configuration = new Configuration
                                                    {
                                                        VsTemplate = VsTemplates.Text,
                                                        VsProject = VsProjects.Text
                                                    };
            Cursor = Cursors.Wait;
            _configuration = configuration;

            var options = new MetroDialogSettings
                          {
                              ColorScheme = MetroDialogColorScheme.Theme
                          };

            MetroDialogOptions = options;
            _controller = await this.ShowProgressAsync("Please wait...", "Repositories are getting migrated.", true, options).ConfigureAwait(true);
            _controller.SetIndeterminate();
            _controller.Canceled += ControllerCanceled;
            RunRepositoryMigration();
            TaskCompleted();
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        private void RunRepositoryMigration()
        {
            var repoPaths = new ConcurrentBag<string>();
            var configuration = _configuration;
            var checkedItems = GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToVsTs);
            var gitCommands = new GitCommands();
            var migrate = new MigrateRepository(_applicationSettings, _templates, _projects, gitCommands, configuration);

            var repositoriesToMigrate = checkedItems as IList<GitRepositoryObservableCollectionItem> ?? checkedItems.ToList();
            try
            {
                Parallel.ForEach(repositoriesToMigrate, checkedItem => checkedItem.MigrationSuccessful = false);

                repositoriesToMigrate.Take(5)
                                     .ToList()
                                     .ForEach(checkedItem =>
                                              {
                                                  var response = migrate.ValueFor(checkedItem.Repository);
                                                  if (response.Code != 200)
                                                  {
                                                      File.AppendAllText("C:/temp/GitVSTSMigration.txt", $@"{response.Value}{Environment.NewLine}");
                                                  }
                                                  else
                                                  {
                                                      checkedItem.MigrationSuccessful = true;
                                                      repoPaths.Add(response.Value);
                                                  }
                                              });
                var restRepositoriesToMigrate = repositoriesToMigrate.Skip(5).ToList();
                Parallel.ForEach(restRepositoriesToMigrate, checkedItem =>
                                                            {
                                                                var response = migrate.ValueFor(checkedItem.Repository);
                                                                if (response.Code != 200)
                                                                {
                                                                    File.AppendAllText("C:/temp/GitVSTSMigration.txt", $@"{response.Value}{Environment.NewLine}");
                                                                }
                                                                else
                                                                {
                                                                    checkedItem.MigrationSuccessful = true;
                                                                    repoPaths.Add(response.Value);
                                                                }
                                                            });
            }
            catch (Exception ex)
            {
                File.AppendAllText("C:/temp/GitVSTSMigration.txt", $@"{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }
            finally
            {
                if (_applicationSettings.DeleteTempRepos)
                {
                    foreach (var repoPath in repoPaths)
                    {
                        if (!Directory.Exists(repoPath))
                        {
                            continue;
                        }

                        try
                        {
                            CleanUpDirectory(repoPath);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("C:/temp/GitVSTSMigration.txt", $@"{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                        }
                    }
                }

                _result = new KeyValuePair<string, string>("Finished", $"All {repositoriesToMigrate.Count} repositories were migrated.");
            }
        }

        private void CleanUpDirectory([NotNull] string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }


            var dir = new DirectoryInfo(path);

            foreach (var fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (var di in dir.GetDirectories())
            {
                CleanUpDirectory(di.FullName);
                di.Delete();
            }
        }

        /// <summary>
        /// </summary>
        private void RefreshMigrationRepos()
        {
            _migrationFailedRepos.Clear();
            _migrationSuccessRepos.Clear();

            GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToVsTs && !attribute.MigrationSuccessful)
                                             .ToList()
                                             .ForEach(repo => _migrationFailedRepos.Add(repo));

            GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToVsTs && attribute.MigrationSuccessful)
                                             .ToList()
                                             .ForEach(repo => _migrationSuccessRepos.Add(repo));

            if (_migrationFailedRepos.Any())
            {
                MigrationFailedTab.Background = _accentColorBrush;
            }

            if (_migrationSuccessRepos.Any())
            {
                SuccessfulTab.Background = _accentColorBrush;
            }
        }


        private void TaskCompleted()
        {
            _controller.CloseAsync();
            _controller.Closed += ControllerClosed;
        }

        private void ControllerClosed(object sender, EventArgs e)
        {
            _dialogService.ShowMessage(_result.Key, _result.Value);
            RefreshMigrationRepos();
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            TaskbarItemInfo.ProgressValue = 1;
            Cursor = Cursors.Arrow;
        }

        private void ControllerCanceled(object sender, EventArgs e)
        {
            _controller.CloseAsync();
            _controller.Closed += ControllerClosed;
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

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (_applicationSettings.GitSourceType)
            {
                case "users":
                    SourceSwitch.IsChecked = false;
                    break;
                // ReSharper disable once StringLiteralTypo
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
                _dialogService.ShowMessage("Path Error", "Path does not contain a 'git.exe'");
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
                _dialogService.ShowMessage("Path Error", "Path does not contain a 'git.exe'");
            }
        }

        private void TabOnGotFocus(object sender, RoutedEventArgs e)
        {
            var current = (TabItem) sender;
            current.Background = GitTab.Background;
        }

        private void AboutWindowClick(object sender, RoutedEventArgs e)
        {
            var assembly = typeof(MainWindow).Assembly;
            IAboutWindowContent aboutWindowContent = new AboutWindowContent(assembly, $@"{AppDomain.CurrentDomain.BaseDirectory}\b.png");

            var aboutWindow = new AboutWindow
                              {
                                  DataContext = new AboutViewModel(aboutWindowContent, _themeManagerHelper)
                              };

            aboutWindow.ShowDialog();
        }

        #endregion Window Methods

        #region Fly-out

        private void ToggleSettingsFlyOutClick(object sender, RoutedEventArgs e)
        {
            ToggleFlyOut(0);
        }

        private void ToggleFlyOut(int index, bool stayOpen = false)
        {
            var activeFlyOut = (Flyout) Flyouts.Items[index];
            if (activeFlyOut == null)
            {
                return;
            }

            foreach (
                var nonactiveFlyOut in
                Flyouts.Items.Cast<Flyout>()
                       .Where(nonactiveFlyOut => nonactiveFlyOut.IsOpen && nonactiveFlyOut.Name != activeFlyOut.Name))
            {
                nonactiveFlyOut.IsOpen = false;
            }

            activeFlyOut.IsOpen = activeFlyOut.IsOpen && stayOpen || !activeFlyOut.IsOpen;
        }

        #endregion Fly-out
    }
}