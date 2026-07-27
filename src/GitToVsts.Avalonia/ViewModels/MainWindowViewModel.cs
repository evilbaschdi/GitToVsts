using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using EvilBaschdi.About.Avalonia;
using EvilBaschdi.Core.Logging;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace GitToVsts.Avalonia.ViewModels;

/// <summary>
///     Main window view model class containing all properties, bindings, and commands for the Git to Azure DevOps
///     migration UI.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly IGitAvatar _gitAvatar;
    private readonly IGitRepositories _gitRepositories;
    private readonly IGitUser _gitUser;
    private readonly IProjects _projects;
    private readonly ITemplates _templates;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMainWindowByApplicationLifetime _mainWindowLifetime;

    private string _gitUsername;
    private string _gitPersonalAccessToken;
    private string _gitSource;
    private bool _gitSourceTypeSwitch;
    private string _tempPath;
    private string _gitBinPath;
    private bool _deleteTempRepos;

    private string _devOpsUsername;
    private string _devOpsPersonalAccessToken;
    private string _devOpsSource;
    private string _devOpsProjectCollection;

    private string _loggingPath;

    /// <summary>
    ///     Constructor for the MainWindowViewModel, injecting core dependencies.
    /// </summary>
    /// <param name="applicationSettings">Application settings provider.</param>
    /// <param name="gitAvatar">Git avatar downloader service.</param>
    /// <param name="gitRepositories">Git repositories retriever service.</param>
    /// <param name="gitUser">Git user details retriever service.</param>
    /// <param name="projects">Azure DevOps projects service.</param>
    /// <param name="templates">Azure DevOps process templates service.</param>
    /// <param name="serviceProvider">Dependency injection service provider.</param>
    /// <param name="mainWindowLifetime">Application lifetime window retriever.</param>
    /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
    public MainWindowViewModel(
        IApplicationSettings applicationSettings,
        IGitAvatar gitAvatar,
        IGitRepositories gitRepositories,
        IGitUser gitUser,
        IProjects projects,
        ITemplates templates,
        IServiceProvider serviceProvider,
        IMainWindowByApplicationLifetime mainWindowLifetime)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        _gitAvatar = gitAvatar ?? throw new ArgumentNullException(nameof(gitAvatar));
        _gitRepositories = gitRepositories ?? throw new ArgumentNullException(nameof(gitRepositories));
        _gitUser = gitUser ?? throw new ArgumentNullException(nameof(gitUser));
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _templates = templates ?? throw new ArgumentNullException(nameof(templates));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _mainWindowLifetime = mainWindowLifetime ?? throw new ArgumentNullException(nameof(mainWindowLifetime));

        // Commands
        GitLoginCommand = ReactiveCommand.CreateFromTask(GitLoginAsync);
        DevOpsLoginCommand = ReactiveCommand.CreateFromTask(DevOpsLoginAsync);
        RunMigrationCommand = ReactiveCommand.CreateFromTask(RunMigrationAsync);
        BrowseTempPathCommand = ReactiveCommand.CreateFromTask(BrowseTempPathAsync);
        BrowseGitPathCommand = ReactiveCommand.CreateFromTask(BrowseGitPathAsync);
        BrowseLoggingPathCommand = ReactiveCommand.CreateFromTask(BrowseLoggingPathAsync);
        CheckAllReposCommand = ReactiveCommand.Create(CheckAllRepos);
        UpdateCheckedReposCountCommand = ReactiveCommand.Create(UpdateCheckedReposCount);
        AboutWindowCommand = ReactiveCommand.CreateFromTask(OpenAboutWindowAsync);
        ToggleSettingsFlyoutCommand = ReactiveCommand.Create(() => SettingsFlyoutIsOpen = !SettingsFlyoutIsOpen);

        LoadSettings();
    }

    #region Properties

    /// <summary>
    ///     Gets or sets the Git username.
    /// </summary>
    public string GitUsername
    {
        get => _gitUsername;
        set
        {
            this.RaiseAndSetIfChanged(ref _gitUsername, value);
            _applicationSettings.GitUser = value;
            ValidateGitInput();
        }
    }

    /// <summary>
    ///     Gets or sets the Git personal access token (PAT).
    /// </summary>
    public string GitPersonalAccessToken
    {
        get => _gitPersonalAccessToken;
        set
        {
            this.RaiseAndSetIfChanged(ref _gitPersonalAccessToken, value);
            _applicationSettings.GitPersonalAccessToken = value;
            ValidateGitInput();
        }
    }

    /// <summary>
    ///     Gets or sets the Git source name (user or organization).
    /// </summary>
    public string GitSource
    {
        get => _gitSource;
        set
        {
            this.RaiseAndSetIfChanged(ref _gitSource, value);
            _applicationSettings.GitSource = value;
            ValidateGitInput();
        }
    }

    /// <summary>
    ///     Gets or sets whether the Git source type is an Organization (true) or User (false).
    /// </summary>
    public bool GitSourceTypeSwitch
    {
        get => _gitSourceTypeSwitch;
        set
        {
            this.RaiseAndSetIfChanged(ref _gitSourceTypeSwitch, value);
            _applicationSettings.GitSourceType = value ? "orgs" : "users";
        }
    }

    /// <summary>
    ///     Gets or sets the temporary path used for cloning repositories.
    /// </summary>
    public string TempPath
    {
        get => _tempPath;
        set
        {
            this.RaiseAndSetIfChanged(ref _tempPath, value);
            if (SanitizedExtensions.SanitizedDirectoryExists(value))
            {
                _applicationSettings.TempPath = value;
            }
        }
    }

    /// <summary>
    ///     Gets or sets the bin path of the local git installation.
    /// </summary>
    public string GitBinPath
    {
        get => _gitBinPath;
        set
        {
            this.RaiseAndSetIfChanged(ref _gitBinPath, value);
            if (SanitizedExtensions.SanitizedDirectoryExists(value) && SanitizedExtensions.SanitizedFileExists(Path.Combine(value, "git.exe")))
            {
                _applicationSettings.GitBinPath = value;
            }
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether cloned repositories should be cleaned up after migration.
    /// </summary>
    public bool DeleteTempRepos
    {
        get => _deleteTempRepos;
        set
        {
            this.RaiseAndSetIfChanged(ref _deleteTempRepos, value);
            _applicationSettings.DeleteTempRepos = value;
        }
    }

    /// <summary>
    ///     Gets or sets the Azure DevOps username.
    /// </summary>
    public string DevOpsUsername
    {
        get => _devOpsUsername;
        set
        {
            this.RaiseAndSetIfChanged(ref _devOpsUsername, value);
            _applicationSettings.DevOpsUser = value;
            ValidateDevOpsInput();
        }
    }

    /// <summary>
    ///     Gets or sets the Azure DevOps personal access token (PAT).
    /// </summary>
    public string DevOpsPersonalAccessToken
    {
        get => _devOpsPersonalAccessToken;
        set
        {
            this.RaiseAndSetIfChanged(ref _devOpsPersonalAccessToken, value);
            _applicationSettings.DevOpsPersonalAccessToken = value;
            ValidateDevOpsInput();
        }
    }

    /// <summary>
    ///     Gets or sets the Azure DevOps target server/source URI.
    /// </summary>
    public string DevOpsSource
    {
        get => _devOpsSource;
        set
        {
            this.RaiseAndSetIfChanged(ref _devOpsSource, value);
            _applicationSettings.DevOpsSource = value;
            ValidateDevOpsInput();
        }
    }

    /// <summary>
    ///     Gets or sets the Azure DevOps project collection name.
    /// </summary>
    public string DevOpsProjectCollection
    {
        get => _devOpsProjectCollection;
        set
        {
            this.RaiseAndSetIfChanged(ref _devOpsProjectCollection, value);
            _applicationSettings.DevOpsProjectCollection = value;
        }
    }

    /// <summary>
    ///     Gets or sets the selected Azure DevOps target project.
    /// </summary>
    public string SelectedDevOpsProject
    {
        get => field;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            if (value != null)
            {
                _applicationSettings.DevOpsProject = value;
                ValidateRunInput();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the selected Azure DevOps process template.
    /// </summary>
    public string SelectedDevOpsTemplate
    {
        get => field;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            if (value != null)
            {
                ValidateRunInput();
            }
        }
    }

    /// <summary>
    ///     Gets or sets the output folder path for log files.
    /// </summary>
    public string LoggingPath
    {
        get => _loggingPath;
        set
        {
            this.RaiseAndSetIfChanged(ref _loggingPath, value);
            if (SanitizedExtensions.SanitizedDirectoryExists(value))
            {
                _applicationSettings.LoggingPath = value;
            }
        }
    }

    /// <summary>
    ///     Gets or sets the text displaying how many repositories are chosen to migrate.
    /// </summary>
    public string RepoLabelText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Choose repositories to migrate";

    /// <summary>
    ///     Gets or sets the collection of available Git repositories.
    /// </summary>
    public ObservableCollection<GitRepositoryObservableCollectionItem> GitRepositoriesCollection
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <summary>
    ///     Gets or sets the collection of repositories whose migration failed.
    /// </summary>
    public ObservableCollection<GitRepositoryObservableCollectionItem> MigrationFailedRepos
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <summary>
    ///     Gets or sets the collection of repositories whose migration succeeded.
    /// </summary>
    public ObservableCollection<GitRepositoryObservableCollectionItem> MigrationSuccessRepos
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <summary>
    ///     Gets or sets the list of Azure DevOps projects available for migration.
    /// </summary>
    public ObservableCollection<string> DevOpsProjects
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <summary>
    ///     Gets or sets the list of Azure DevOps process templates available.
    /// </summary>
    public ObservableCollection<string> DevOpsTemplates
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    /// <summary>
    ///     Gets or sets a value indicating whether the Repositories tab is enabled.
    /// </summary>
    public bool RepoTabIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Azure DevOps tab is enabled.
    /// </summary>
    public bool DevOpsTabIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Run tab is enabled.
    /// </summary>
    public bool RunTabIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Failed tab is enabled.
    /// </summary>
    public bool MigrationFailedTabIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Successful tab is enabled.
    /// </summary>
    public bool SuccessfulTabIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Git Login button is enabled.
    /// </summary>
    public bool GitLoginIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the DevOps Login button is enabled.
    /// </summary>
    public bool DevOpsLoginIsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets the Git user avatar image source bitmap.
    /// </summary>
    public Bitmap GitAvatarSource
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Git avatar is visible.
    /// </summary>
    public bool GitAvatarIsVisible
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the Git Login form/button is visible.
    /// </summary>
    public bool GitLoginIsVisible
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>
    ///     Gets or sets the selected tab index of the main TabControl.
    /// </summary>
    public int SelectedIndex
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the view model is performing a long-running background operation.
    /// </summary>
    public bool IsBusy
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets the message to display on the progress overlay.
    /// </summary>
    public string BusyMessage
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the settings flyout pane is currently open.
    /// </summary>
    public bool SettingsFlyoutIsOpen
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    #endregion Properties

    #region Commands Decl

    /// <summary>
    ///     Gets the command to trigger Git authentication.
    /// </summary>
    public ICommand GitLoginCommand { get; }

    /// <summary>
    ///     Gets the command to trigger DevOps projects and templates retrieval.
    /// </summary>
    public ICommand DevOpsLoginCommand { get; }

    /// <summary>
    ///     Gets the command to run the migration process.
    /// </summary>
    public ICommand RunMigrationCommand { get; }

    /// <summary>
    ///     Gets the command to open a folder dialog for the temporary clone directory.
    /// </summary>
    public ICommand BrowseTempPathCommand { get; }

    /// <summary>
    ///     Gets the command to open a folder dialog for the local Git binary directory.
    /// </summary>
    public ICommand BrowseGitPathCommand { get; }

    /// <summary>
    ///     Gets the command to open a folder dialog for the logging output directory.
    /// </summary>
    public ICommand BrowseLoggingPathCommand { get; }

    /// <summary>
    ///     Gets the command to check all repositories in the list.
    /// </summary>
    public ICommand CheckAllReposCommand { get; }

    /// <summary>
    ///     Gets the command to trigger recalculation of selected repositories.
    /// </summary>
    public ICommand UpdateCheckedReposCountCommand { get; }

    /// <summary>
    ///     Gets the command to show the custom About Window dialog.
    /// </summary>
    public ICommand AboutWindowCommand { get; }

    /// <summary>
    ///     Gets the command to toggle the visibility of the Settings flyout panel.
    /// </summary>
    public ICommand ToggleSettingsFlyoutCommand { get; }

    #endregion Commands Decl

    #region Command Handlers

    private void LoadSettings()
    {
        _gitUsername = _applicationSettings.GitUser;
        _gitPersonalAccessToken = _applicationSettings.GitPersonalAccessToken;
        _gitSource = _applicationSettings.GitSource;
        _devOpsUsername = _applicationSettings.DevOpsUser;
        _devOpsPersonalAccessToken = _applicationSettings.DevOpsPersonalAccessToken;
        _devOpsSource = _applicationSettings.DevOpsSource;
        _devOpsProjectCollection = _applicationSettings.DevOpsProjectCollection;
        _loggingPath = _applicationSettings.LoggingPath;
        _tempPath = _applicationSettings.TempPath;
        _gitBinPath = _applicationSettings.GitBinPath;
        _deleteTempRepos = _applicationSettings.DeleteTempRepos;
        _gitSourceTypeSwitch = _applicationSettings.GitSourceType == "orgs";

        ValidateGitInput();
        ValidateDevOpsInput();
    }

    private void ValidateGitInput()
    {
        GitLoginIsEnabled = !string.IsNullOrWhiteSpace(GitUsername) &&
                            !string.IsNullOrWhiteSpace(GitPersonalAccessToken) &&
                            !string.IsNullOrWhiteSpace(GitSource);
    }

    private void ValidateDevOpsInput()
    {
        DevOpsLoginIsEnabled = !string.IsNullOrWhiteSpace(DevOpsPersonalAccessToken) &&
                               !string.IsNullOrWhiteSpace(DevOpsSource);
    }

    private void ValidateRunInput()
    {
        var isDefaultProject = SelectedDevOpsProject != null && SelectedDevOpsProject.Contains("(default)");
        RunTabIsEnabled = !string.IsNullOrWhiteSpace(SelectedDevOpsProject) && (!isDefaultProject || !string.IsNullOrWhiteSpace(SelectedDevOpsTemplate));
    }

    private async Task GitLoginAsync()
    {
        IsBusy = true;
        BusyMessage = "Authenticating with GitHub and fetching repositories.";

        try
        {
            GitUser user = null;
            await Task.Run(() => user = _gitUser.Value).ConfigureAwait(true);

            if (user != null)
            {
                await Task.Run(LoadGitRepositoryList).ConfigureAwait(true);

                var avatarBytes = await _gitAvatar.ValueFor(user).ConfigureAwait(true);
                if (avatarBytes is { Length: > 0 })
                {
                    using var ms = new MemoryStream(avatarBytes);
                    GitAvatarSource = new Bitmap(ms);
                    GitAvatarIsVisible = true;
                    GitLoginIsVisible = false;
                }

                RepoTabIsEnabled = true;
                SelectedIndex = 1;
            }
            else
            {
                GitUsername = "";
                GitPersonalAccessToken = "";
            }
        }
        catch (Exception)
        {
            // Handle error
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadGitRepositoryList()
    {
        var repos = _gitRepositories.Value;
        var collection = new ObservableCollection<GitRepositoryObservableCollectionItem>();

        var i = 1;
        foreach (var repo in repos)
        {
            collection.Add(new GitRepositoryObservableCollectionItem
                           {
                               DisplayName = $"{i++}_{repo.Name}",
                               Repository = repo
                           });
        }

        GitRepositoriesCollection = collection;
        MigrationFailedRepos.Clear();
        MigrationSuccessRepos.Clear();
    }

    private async Task DevOpsLoginAsync()
    {
        IsBusy = true;
        BusyMessage = "Fetching projects and templates from Azure DevOps.";

        try
        {
            await Task.Run(() =>
                           {
                               var projects = _projects.Value;
                               var templates = _templates.Value;

                               DevOpsProjects.Clear();
                               DevOpsProjects.Add("one project per repo (default)");
                               if (projects?.Value != null)
                               {
                                   foreach (var project in projects.Value)
                                   {
                                       DevOpsProjects.Add(project.Name);
                                   }
                               }

                               DevOpsTemplates.Clear();
                               if (templates?.Value != null)
                               {
                                   string defaultTemplateName = null;
                                   foreach (var template in templates.Value)
                                   {
                                       DevOpsTemplates.Add(template.Name);
                                       if (template.IsDefault)
                                       {
                                           defaultTemplateName = template.Name;
                                       }
                                   }

                                   if (!string.IsNullOrWhiteSpace(defaultTemplateName))
                                   {
                                       SelectedDevOpsTemplate = defaultTemplateName;
                                   }
                                   else if (DevOpsTemplates.Count > 0)
                                   {
                                       SelectedDevOpsTemplate = DevOpsTemplates[0];
                                   }
                               }

                               MigrationFailedTabIsEnabled = true;
                               SuccessfulTabIsEnabled = true;
                               DevOpsLoginIsEnabled = false;
                               SelectedIndex = 2;
                           }).ConfigureAwait(true);
        }
        catch (Exception)
        {
            // Handle error
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CheckAllRepos()
    {
        foreach (var item in GitRepositoriesCollection)
        {
            item.MigrateToDevOps = true;
        }

        // Force UI update
        var temp = GitRepositoriesCollection;
        GitRepositoriesCollection = null;
        GitRepositoriesCollection = temp;

        UpdateCheckedReposCount();
    }

    /// <summary>
    ///     Updates the count of checked repositories and enables/disables the DevOps tab accordingly.
    /// </summary>
    public void UpdateCheckedReposCount()
    {
        var checkedItemsCount = GitRepositoriesCollection.Count(attribute => attribute.MigrateToDevOps);
        RepoLabelText = $"Repositories chosen to migrate: {checkedItemsCount}";
        DevOpsTabIsEnabled = checkedItemsCount != 0;
    }

    private async Task BrowseTempPathAsync()
    {
        var window = _mainWindowLifetime.Value;
        if (window is not null)
        {
            var topLevel = TopLevel.GetTopLevel(window);
            if (topLevel != null)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                                                                                   {
                                                                                       Title = "Select Working Directory",
                                                                                       AllowMultiple = false
                                                                                   });
                if (folders.Count > 0)
                {
                    TempPath = folders[0].Path.LocalPath;
                }
            }
        }
    }

    private async Task BrowseGitPathAsync()
    {
        var window = _mainWindowLifetime.Value;
        if (window is not null)
        {
            var topLevel = TopLevel.GetTopLevel(window);
            if (topLevel != null)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                                                                                   {
                                                                                       Title = "Select Git Bin Directory",
                                                                                       AllowMultiple = false
                                                                                   });
                if (folders.Count > 0)
                {
                    var path = folders[0].Path.LocalPath;
                    if (File.Exists(Path.Combine(path, "git.exe")))
                    {
                        GitBinPath = path;
                    }
                }
            }
        }
    }

    private async Task BrowseLoggingPathAsync()
    {
        var window = _mainWindowLifetime.Value;
        if (window is not null)
        {
            var topLevel = TopLevel.GetTopLevel(window);
            if (topLevel != null)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                                                                                   {
                                                                                       Title = "Select Logging Directory",
                                                                                       AllowMultiple = false
                                                                                   });
                if (folders.Count > 0)
                {
                    LoggingPath = folders[0].Path.LocalPath;
                }
            }
        }
    }

    private async Task OpenAboutWindowAsync()
    {
        var aboutWindow = _serviceProvider.GetRequiredService<AboutWindow>();
        var mainWindow = _mainWindowLifetime.Value;
        if (mainWindow is not null)
        {
            await aboutWindow.ShowDialog(mainWindow);
        }
    }

    private async Task RunMigrationAsync()
    {
        IsBusy = true;
        BusyMessage = "Repositories are getting migrated.";

        try
        {
            await Task.Run(RunRepositoryMigration).ConfigureAwait(true);
            RefreshMigrationRepos();
            SelectedIndex = MigrationFailedRepos.Count > 0 ? 4 : 5;
        }
        catch (Exception)
        {
            // Handle error
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void RunRepositoryMigration()
    {
        var logFilePathWithInterval = new LogFilePathWithInterval();
        var logPath = logFilePathWithInterval.ValueFor(_applicationSettings.LoggingPath, "GitToVsts", LoggingFileInterval.None, DateTime.Now, "txt");
        ILogger logger = new SimpleFileLogger(logPath, LogLevel.Information, new object(), "GitToVsts Log");
        var repoPaths = new ConcurrentBag<string>();
        var configuration = new Configuration
                            {
                                DevOpsTemplate = SelectedDevOpsTemplate,
                                DevOpsProject = SelectedDevOpsProject
                            };
        var checkedItems = GitRepositoriesCollection.Where(attribute => attribute.MigrateToDevOps);
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
                                                  logger.LogInformation("{ResponseValue}", response.Value);
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
                                                                logger.LogInformation("{ResponseValue}", response.Value);
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
            logger.LogError(ex, "Error during migration");
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
                        logger.LogError(ex, "Error during cleanup");
                    }
                }
            }
        }
    }

    private void RefreshMigrationRepos()
    {
        MigrationFailedRepos.Clear();
        MigrationSuccessRepos.Clear();

        foreach (var repo in GitRepositoriesCollection.Where(attribute => attribute.MigrateToDevOps && !attribute.MigrationSuccessful))
        {
            MigrationFailedRepos.Add(repo);
        }

        foreach (var repo in GitRepositoriesCollection.Where(attribute => attribute.MigrateToDevOps && attribute.MigrationSuccessful))
        {
            MigrationSuccessRepos.Add(repo);
        }
    }

    private static void CleanUpDirectory(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

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

    #endregion Command Handlers
}