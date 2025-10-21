using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using ControlzEx.Theming;
using EvilBaschdi.About.Core;
using EvilBaschdi.About.Core.Models;
using EvilBaschdi.About.Wpf;
using EvilBaschdi.Core;
using EvilBaschdi.Core.Settings.ByMachineAndUser;
using EvilBaschdi.Core.Wpf;
using EvilBaschdi.Core.Wpf.Browsers;
using EvilBaschdi.Core.Wpf.FlyOut;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Internal.TeamServices;
using GitToVsts.Model;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GitToVsts;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : MetroWindow
{
    private readonly Brush _accentColorBrush;
    private readonly IApplicationSettings _applicationSettings;
    private readonly ICurrentFlyOuts _currentFlyOuts;
    private readonly ObservableCollection<GitRepositoryObservableCollectionItem> _migrationFailedRepos = [];
    private readonly ObservableCollection<GitRepositoryObservableCollectionItem> _migrationSuccessRepos = [];
    private readonly IToggleFlyOut _toggleFlyOut;

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
        IAppSettingsFromJsonFile appSettingsFromJsonFile = new AppSettingsFromJsonFile();
        IAppSettingsFromJsonFileByMachineAndUser appSettingsFromJsonFileByMachineAndUser = new AppSettingsFromJsonFileByMachineAndUser();
        IAppSettingByKey appSettingByKey = new AppSettingByKey(appSettingsFromJsonFile, appSettingsFromJsonFileByMachineAndUser);
        _applicationSettings = new ApplicationSettings(appSettingByKey);

        IApplicationStyle applicationStyle = new ApplicationStyle();
        IApplicationLayout applicationLayout = new ApplicationLayout();
        applicationStyle.Run();
        applicationLayout.RunFor((true, false));
        _currentFlyOuts = new CurrentFlyOuts();
        _toggleFlyOut = new ToggleFlyOut();

        var accentColor = WindowsThemeHelper.GetWindowsAccentColor();
        if (accentColor.HasValue)
        {
            _accentColorBrush = new SolidColorBrush(accentColor.Value);
        }

        Load();
    }

    /// <summary>
    ///     ObservableCollection to contain GitRepositories.
    /// </summary>
    private ObservableCollection<GitRepositoryObservableCollectionItem> GitRepositoryObservableCollection { get; set; }

    #region Fly-out

    private void ToggleSettingsFlyOutClick(object sender, RoutedEventArgs e)
    {
        var currentFlyOutsModel = _currentFlyOuts.ValueFor(Flyouts, 0);
        _toggleFlyOut.RunFor(currentFlyOutsModel);
    }

    #endregion Fly-out

    #region GitTab

    private void LoadGitRepositoryList()
    {
        var collection = GetGitRepositoryObservableCollection();
        Application.Current.Dispatcher.Invoke(() =>
        {
            GitRepositoryObservableCollection = collection;
            GitRepositoryObservableCollectionBox.SetCurrentValue(ItemsControl.ItemsSourceProperty, GitRepositoryObservableCollection);
            GitRepositoryMigrationFailedObservableCollectionBox.SetCurrentValue(ItemsControl.ItemsSourceProperty, _migrationFailedRepos);
            GitRepositoryMigrationSuccessObservableCollectionBox.SetCurrentValue(ItemsControl.ItemsSourceProperty, _migrationSuccessRepos);
        });
    }

    private ObservableCollection<GitRepositoryObservableCollectionItem> GetGitRepositoryObservableCollection()
    {
        var collection = new ObservableCollection<GitRepositoryObservableCollectionItem>();

        var i = 1;
        foreach (var gitRepositoryObservableCollectionItem in
                 _gitRepositories.Value
                                 .Select(repository
                                             => new GitRepositoryObservableCollectionItem
                                                {
                                                    DisplayName = $"{i++}_{repository.Name}",
                                                    Repository = repository
                                                }))
        {
            collection.Add(gitRepositoryObservableCollectionItem);
        }

        return collection;
    }

    private async void GitLoginOnClick(object sender, RoutedEventArgs e)
    {
        _applicationSettings.GitUser = GitUsername.Text;
        _applicationSettings.GitPersonalAccessToken = GitPersonalAccessToken.Password;

        var options = new MetroDialogSettings
                      {
                          ColorScheme = MetroDialogColorScheme.Theme
                      };

        _controller = await this.ShowProgressAsync("Please wait...", "Authenticating with GitHub and fetching repositories.", true, options).ConfigureAwait(true);
        _controller.SetIndeterminate();

        var getGitUser = new GetGitUser(_applicationSettings);
        GitUser user = null;
        await Task.Run(() => user = getGitUser.Value).ConfigureAwait(true);

        if (user != null)
        {
            _applicationSettings.GitSource = GitSource.Text;
            _gitRepositories = new GetGitRepositories(_applicationSettings);
            await Task.Run(LoadGitRepositoryList).ConfigureAwait(true);

            var convertGitAvatar = new ConvertGitAvatar();
            var avatar = await convertGitAvatar.ValueFor(user).ConfigureAwait(true);
            GitAvatar.SetCurrentValue(Image.SourceProperty, avatar);
            GitAvatar.SetCurrentValue(VisibilityProperty, Visibility.Visible);
            GitLogin.SetCurrentValue(VisibilityProperty, Visibility.Hidden);

            await _controller.CloseAsync().ConfigureAwait(true);
            await this.ShowMessageAsync("Successful", $"'{user.Login}' was successfully authenticated {Environment.NewLine}Please switch to 'Repositories'").ConfigureAwait(true);
            RepoTab.SetCurrentValue(IsEnabledProperty, true);
            RepoTab.SetCurrentValue(BackgroundProperty, _accentColorBrush);
            MainTabControl.SetCurrentValue(Selector.SelectedIndexProperty, 1);
        }
        else
        {
            await _controller.CloseAsync().ConfigureAwait(true);
            _applicationSettings.GitUser = "";
            _applicationSettings.GitPersonalAccessToken = "";
            await this.ShowMessageAsync("Error", "Authentication failed. Please check your credentials.").ConfigureAwait(true);
        }
    }

    private void SourceType(object sender, RoutedEventArgs e)
    {
        if (_overrideProtection == 0)
        {
            return;
        }

        var toggleSwitch = (ToggleSwitch)sender;
        // ReSharper disable once StringLiteralTypo
        _applicationSettings.GitSourceType = toggleSwitch.IsOn ? "orgs" : "users";
    }

    private void ValidateGitTextBoxesOnTextChanged(object sender, EventArgs e)
    {
        GitLogin.SetCurrentValue(IsEnabledProperty, !string.IsNullOrWhiteSpace(GitUsername.Text) &&
                                                    !string.IsNullOrWhiteSpace(GitPersonalAccessToken.Password) &&
                                                    !string.IsNullOrWhiteSpace(GitSource.Text));
    }

    #endregion GitTab

    #region RepoTab

    private void MigrateToDevOpsOnClick(object sender, RoutedEventArgs e)
    {
        SetRepoLabelContent();
    }

    private void MigrateAllOnClick(object sender, RoutedEventArgs e)
    {
        foreach (var gitRepositoryObservableCollectionItem in GitRepositoryObservableCollection)
        {
            gitRepositoryObservableCollectionItem.MigrateToDevOps = true;
        }

        GitRepositoryObservableCollectionBox.SetCurrentValue(ItemsControl.ItemsSourceProperty, null);
        GitRepositoryObservableCollectionBox.SetCurrentValue(ItemsControl.ItemsSourceProperty, GitRepositoryObservableCollection);

        SetRepoLabelContent();
    }

    private void SetRepoLabelContent()
    {
        var checkedItemsCount = GitRepositoryObservableCollection.Count(attribute => attribute.MigrateToDevOps);
        RepoLabel.SetCurrentValue(ContentProperty, $"Repositories chosen to migrate: {checkedItemsCount}");
        DevOpsTab.SetCurrentValue(IsEnabledProperty, checkedItemsCount != 0);
        DevOpsTab.SetCurrentValue(BackgroundProperty, DevOpsTab.IsEnabled ? _accentColorBrush : GitTab.Background);
    }

    #endregion RepoTab

    #region DevOpsTab

    private async void DevOpsLoginOnClick(object sender, RoutedEventArgs e)
    {
        _applicationSettings.DevOpsUser = DevOpsUsername.Text;
        _applicationSettings.DevOpsPersonalAccessToken = DevOpsPersonalAccessToken.Password;
        _applicationSettings.DevOpsSource = DevOpsSource.Text;
        _applicationSettings.DevOpsProjectCollection = DevOpsProjectCollection.Text;

        if (!string.IsNullOrWhiteSpace(DevOpsProjects.Text))
        {
            _applicationSettings.DevOpsProject = DevOpsProjects.Text;
        }

        var options = new MetroDialogSettings
                      {
                          ColorScheme = MetroDialogColorScheme.Theme
                      };

        _controller = await this.ShowProgressAsync("Please wait...", "Fetching projects and templates from Azure DevOps.", true, options).ConfigureAwait(true);
        _controller.SetIndeterminate();

        var projectsFetcher = new GetProjects(_applicationSettings);
        var templatesFetcher = new GetTemplates(_applicationSettings);

        try
        {
            await Task.Run(() =>
            {
                var projects = projectsFetcher.Value;
                var templates = templatesFetcher.Value;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _projects = projectsFetcher;
                    _templates = templatesFetcher;

                    DevOpsProjects.Items.Clear();
                    DevOpsProjects.Items.Add("one project per repo (default)");
                    if (projects?.Value != null)
                    {
                        foreach (var project in projects.Value)
                        {
                            DevOpsProjects.Items.Add(project.Name);
                        }
                    }

                    DevOpsTemplates.Items.Clear();
                    if (templates?.Value != null)
                    {
                        string defaultTemplateName = null;
                        foreach (var template in templates.Value)
                        {
                            DevOpsTemplates.Items.Add(template.Name);
                            if (template.IsDefault)
                            {
                                defaultTemplateName = template.Name;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(defaultTemplateName))
                        {
                            DevOpsTemplates.SetCurrentValue(ComboBox.TextProperty, defaultTemplateName);
                        }
                        else if (DevOpsTemplates.Items.Count > 0)
                        {
                            DevOpsTemplates.SetCurrentValue(Selector.SelectedIndexProperty, 0);
                        }
                    }

                                    DevOpsProjects.SetCurrentValue(IsEnabledProperty, true);

                                    DevOpsTemplates.SetCurrentValue(IsEnabledProperty, true);

                                    MigrationFailedTab.SetCurrentValue(IsEnabledProperty, true);

                                    SuccessfulTab.SetCurrentValue(IsEnabledProperty, true);

                                    DevOpsLogin.SetCurrentValue(IsEnabledProperty, false);

                                    MainTabControl.SetCurrentValue(Selector.SelectedIndexProperty, 2);

                                });

                    
            }).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            await this.ShowMessageAsync("Error", $"Failed to fetch data from Azure DevOps: {ex.Message}").ConfigureAwait(true);
        }
        finally
        {
            await _controller.CloseAsync().ConfigureAwait(true);
        }
    }

    private void DevOpsTemplatesOnDropDownClosed(object sender, EventArgs e)
    {
        var isDefaultProject = DevOpsProjects.Text.Contains("(default)");
        if (string.IsNullOrWhiteSpace(DevOpsProjects.Text) || (isDefaultProject && string.IsNullOrWhiteSpace(DevOpsTemplates.Text)))
        {
            return;
        }

        RunTab.SetCurrentValue(IsEnabledProperty, true);
        RunTab.SetCurrentValue(BackgroundProperty, _accentColorBrush);
    }

    private void DevOpsProjectsOnDropDownClosed(object sender, EventArgs e)
    {
        var isDefaultProject = DevOpsProjects.Text.Contains("(default)");
        if (string.IsNullOrWhiteSpace(DevOpsProjects.Text) || (isDefaultProject && string.IsNullOrWhiteSpace(DevOpsTemplates.Text)))
        {
            return;
        }

        _applicationSettings.DevOpsProject = DevOpsProjects.Text;
        RunTab.SetCurrentValue(IsEnabledProperty, true);
        RunTab.SetCurrentValue(BackgroundProperty, _accentColorBrush);
        MainTabControl.SetCurrentValue(Selector.SelectedIndexProperty, 3);
    }

    private void ValidateDevOpsTextBoxesOnTextChanged(object sender, RoutedEventArgs e)
    {
        DevOpsLogin.SetCurrentValue(IsEnabledProperty, !string.IsNullOrWhiteSpace(DevOpsPersonalAccessToken.Password) &&
                                                   !string.IsNullOrWhiteSpace(DevOpsSource.Text));
    }

    #endregion DevOpsTab

    #region RunTab

    private async void RunMigrationOnClickAsync(object sender, RoutedEventArgs e)
    {
        TaskbarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Indeterminate);
        // ReSharper disable once AsyncConverter.AsyncAwaitMayBeElidedHighlighting
        await RunAsync().ConfigureAwait(true);
    }

    private async Task RunAsync()
    {
        IMigrationConfiguration configuration = new Configuration
                                                {
                                                    DevOpsTemplate = DevOpsTemplates.Text,
                                                    DevOpsProject = DevOpsProjects.Text
                                                };
        SetCurrentValue(CursorProperty, Cursors.Wait);
        _configuration = configuration;

        var options = new MetroDialogSettings
                      {
                          ColorScheme = MetroDialogColorScheme.Theme
                      };

        SetCurrentValue(MetroDialogOptionsProperty, options);
        _controller = await this.ShowProgressAsync("Please wait...", "Repositories are getting migrated.", true, options).ConfigureAwait(true);
        _controller.SetIndeterminate();
        _controller.Canceled += ControllerCanceled;
        await Task.Run(RunRepositoryMigration).ConfigureAwait(true);
        TaskCompleted();
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    private void RunRepositoryMigration()
    {
        var repoPaths = new ConcurrentBag<string>();
        var configuration = _configuration;
        var checkedItems = GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToDevOps);
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
                                                  File.AppendAllText("C:/temp/GitAzureDevOpsMigration.txt", $@"{response.Value}{Environment.NewLine}");
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
                                                                File.AppendAllText("C:/temp/GitAzureDevOpsMigration.txt", $@"{response.Value}{Environment.NewLine}");
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
            File.AppendAllText("C:/temp/GitAzureDevOpsMigration.txt", $@"{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
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
                        File.AppendAllText("C:/temp/GitAzureDevOpsMigration.txt", $@"{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    }
                }
            }

            _result = new("Finished", $"All {repositoriesToMigrate.Count} repositories were migrated.");
        }
    }

    private static void CleanUpDirectory([NotNull] string path)
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
        Application.Current.Dispatcher.Invoke(() =>
        {
            _migrationFailedRepos.Clear();
            _migrationSuccessRepos.Clear();

            GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToDevOps && !attribute.MigrationSuccessful)
                                             .ToList()
                                             .ForEach(repo => _migrationFailedRepos.Add(repo));

            GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToDevOps && attribute.MigrationSuccessful)
                                             .ToList()
                                             .ForEach(repo => _migrationSuccessRepos.Add(repo));

            if (_migrationFailedRepos.Any())
            {
                MigrationFailedTab.SetCurrentValue(BackgroundProperty, _accentColorBrush);
            }

            if (_migrationSuccessRepos.Any())
            {
                SuccessfulTab.SetCurrentValue(BackgroundProperty, _accentColorBrush);
            }
        });
    }

    private void TaskCompleted()
    {
        _controller.CloseAsync();
        _controller.Closed += ControllerClosed;
    }

    private void ControllerClosed(object sender, EventArgs e)
    {
        this.ShowMessageAsync(_result.Key, _result.Value);
        RefreshMigrationRepos();
        TaskbarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Normal);
        TaskbarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressValueProperty, (double)1);
        SetCurrentValue(CursorProperty, Cursors.Arrow);

        if (_migrationFailedRepos.Any())
        {
            MainTabControl.SetCurrentValue(Selector.SelectedIndexProperty, 4);
        }
        else
        {
            MainTabControl.SetCurrentValue(Selector.SelectedIndexProperty, 5);
        }
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
        GitUsername.SetCurrentValue(TextBox.TextProperty, _applicationSettings.GitUser);
        GitPersonalAccessToken.Password = _applicationSettings.GitPersonalAccessToken;
        GitSource.SetCurrentValue(TextBox.TextProperty, _applicationSettings.GitSource);
        DevOpsUsername.SetCurrentValue(TextBox.TextProperty, _applicationSettings.DevOpsUser);
        DevOpsPersonalAccessToken.Password = _applicationSettings.DevOpsPersonalAccessToken;
        DevOpsSource.SetCurrentValue(TextBox.TextProperty, _applicationSettings.DevOpsSource);
        DevOpsProjectCollection.SetCurrentValue(TextBox.TextProperty, _applicationSettings.DevOpsProjectCollection);
        DevOpsProjects.SetCurrentValue(ComboBox.TextProperty, _applicationSettings.DevOpsProject);
        LoggingPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.LoggingPath);
        TempPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.TempPath);
        GitBinPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.GitBinPath);
        CleanUpSwitch.SetCurrentValue(ToggleSwitch.IsOnProperty, _applicationSettings.DeleteTempRepos);

        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (_applicationSettings.GitSourceType)
        {
            case "users":
                SourceSwitch.SetCurrentValue(ToggleSwitch.IsOnProperty, false);
                break;
            // ReSharper disable once StringLiteralTypo
            case "orgs":
                SourceSwitch.SetCurrentValue(ToggleSwitch.IsOnProperty, true);
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
        LoggingPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.LoggingPath);
    }

    private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
    {
        if (SanitizedExtensions.SanitizedDirectoryExists(LoggingPath.Text))
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
        TempPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.TempPath);
    }

    private void TempPathOnLostFocus(object sender, RoutedEventArgs e)
    {
        if (SanitizedExtensions.SanitizedDirectoryExists(TempPath.Text))
        {
            _applicationSettings.TempPath = TempPath.Text;
        }
    }

    private void CleanUp(object sender, RoutedEventArgs e)
    {
        var toggleSwitch = (ToggleSwitch)sender;
        _applicationSettings.DeleteTempRepos = toggleSwitch.IsOn;
    }

    private void BrowseGitPathClick(object sender, RoutedEventArgs e)
    {
        var browser = new ExplorerFolderBrowser
                      {
                          SelectedPath = _applicationSettings.GitBinPath
                      };
        browser.ShowDialog();
        if (SanitizedExtensions.SanitizedFileExists($@"{browser.SelectedPath}\git.exe"))
        {
            _applicationSettings.GitBinPath = browser.SelectedPath;
            GitBinPath.SetCurrentValue(TextBox.TextProperty, _applicationSettings.GitBinPath);
        }
        else
        {
            this.ShowMessageAsync("Path Error", "Path does not contain a 'git.exe'");
        }
    }

    private void GitPathOnLostFocus(object sender, RoutedEventArgs e)
    {
        if (SanitizedExtensions.SanitizedDirectoryExists(GitBinPath.Text) && SanitizedExtensions.SanitizedFileExists($@"{GitBinPath.Text}\git.exe"))
        {
            _applicationSettings.GitBinPath = GitBinPath.Text;
        }
        else
        {
            this.ShowMessageAsync("Path Error", "Path does not contain a 'git.exe'");
        }
    }

    private void TabOnGotFocus(object sender, RoutedEventArgs e)
    {
        var current = (TabItem)sender;
        current.Background = GitTab.Background;
    }

    private void AboutWindowClick(object sender, RoutedEventArgs e)
    {
        ICurrentAssembly currentAssembly = new CurrentAssembly();
        IAboutContent aboutContent = new AboutContent(currentAssembly);
        IAboutViewModel aboutModel = new AboutViewModel(aboutContent);
        IApplyMicaBrush applyMicaBrush = new ApplyMicaBrush();
        IApplicationLayout applicationLayout = new ApplicationLayout();
        var aboutWindow = new AboutWindow(aboutModel, applicationLayout, applyMicaBrush);

        aboutWindow.ShowDialog();
    }

    #endregion Window Methods
}
