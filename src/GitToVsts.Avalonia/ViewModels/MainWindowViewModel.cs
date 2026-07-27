using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using EvilBaschdi.About.Avalonia;
using EvilBaschdi.Core.Avalonia.DependencyInjection;
using EvilBaschdi.Core.Logging;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Internal.TeamServices;
using GitToVsts.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace GitToVsts.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly IGitAvatar _gitAvatar;
    private readonly IGitRepositories _gitRepositories;
    private readonly IGitUser _gitUser;
    private readonly IProjects _projects;
    private readonly ITemplates _templates;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
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
    private string _selectedDevOpsProject;
    private string _selectedDevOpsTemplate;

    private string _loggingPath;

    private ObservableCollection<GitRepositoryObservableCollectionItem> _gitRepositoriesCollection = [];
    private ObservableCollection<GitRepositoryObservableCollectionItem> _migrationFailedRepos = [];
    private ObservableCollection<GitRepositoryObservableCollectionItem> _migrationSuccessRepos = [];
    private ObservableCollection<string> _devOpsProjects = [];
    private ObservableCollection<string> _devOpsTemplates = [];

    private bool _repoTabIsEnabled;
    private bool _devOpsTabIsEnabled;
    private bool _runTabIsEnabled;
    private bool _migrationFailedTabIsEnabled;
    private bool _successfulTabIsEnabled;

    private bool _gitLoginIsEnabled;
    private bool _devOpsLoginIsEnabled;

    private Bitmap _gitAvatarSource;
    private bool _gitAvatarIsVisible;
    private bool _gitLoginIsVisible = true;

    private int _selectedIndex;
    private bool _isBusy;
    private string _busyMessage;
    private bool _settingsFlyoutIsOpen;
    private string _repoLabelText = "Choose repositories to migrate";

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

        _httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();

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

    public bool GitSourceTypeSwitch
    {
        get => _gitSourceTypeSwitch;
        set
        {
            this.RaiseAndSetIfChanged(ref _gitSourceTypeSwitch, value);
            _applicationSettings.GitSourceType = value ? "orgs" : "users";
        }
    }

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

    public bool DeleteTempRepos
    {
        get => _deleteTempRepos;
        set
        {
            this.RaiseAndSetIfChanged(ref _deleteTempRepos, value);
            _applicationSettings.DeleteTempRepos = value;
        }
    }

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

    public string DevOpsProjectCollection
    {
        get => _devOpsProjectCollection;
        set
        {
            this.RaiseAndSetIfChanged(ref _devOpsProjectCollection, value);
            _applicationSettings.DevOpsProjectCollection = value;
        }
    }

    public string SelectedDevOpsProject
    {
        get => _selectedDevOpsProject;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDevOpsProject, value);
            if (value != null)
            {
                _applicationSettings.DevOpsProject = value;
                ValidateRunInput();
            }
        }
    }

    public string SelectedDevOpsTemplate
    {
        get => _selectedDevOpsTemplate;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDevOpsTemplate, value);
            if (value != null)
            {
                ValidateRunInput();
            }
        }
    }

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

    public string RepoLabelText
    {
        get => _repoLabelText;
        set => this.RaiseAndSetIfChanged(ref _repoLabelText, value);
    }

    public ObservableCollection<GitRepositoryObservableCollectionItem> GitRepositoriesCollection
    {
        get => _gitRepositoriesCollection;
        set => this.RaiseAndSetIfChanged(ref _gitRepositoriesCollection, value);
    }

    public ObservableCollection<GitRepositoryObservableCollectionItem> MigrationFailedRepos
    {
        get => _migrationFailedRepos;
        set => this.RaiseAndSetIfChanged(ref _migrationFailedRepos, value);
    }

    public ObservableCollection<GitRepositoryObservableCollectionItem> MigrationSuccessRepos
    {
        get => _migrationSuccessRepos;
        set => this.RaiseAndSetIfChanged(ref _migrationSuccessRepos, value);
    }

    public ObservableCollection<string> DevOpsProjects
    {
        get => _devOpsProjects;
        set => this.RaiseAndSetIfChanged(ref _devOpsProjects, value);
    }

    public ObservableCollection<string> DevOpsTemplates
    {
        get => _devOpsTemplates;
        set => this.RaiseAndSetIfChanged(ref _devOpsTemplates, value);
    }

    public bool RepoTabIsEnabled
    {
        get => _repoTabIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _repoTabIsEnabled, value);
    }

    public bool DevOpsTabIsEnabled
    {
        get => _devOpsTabIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _devOpsTabIsEnabled, value);
    }

    public bool RunTabIsEnabled
    {
        get => _runTabIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _runTabIsEnabled, value);
    }

    public bool MigrationFailedTabIsEnabled
    {
        get => _migrationFailedTabIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _migrationFailedTabIsEnabled, value);
    }

    public bool SuccessfulTabIsEnabled
    {
        get => _successfulTabIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _successfulTabIsEnabled, value);
    }

    public bool GitLoginIsEnabled
    {
        get => _gitLoginIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _gitLoginIsEnabled, value);
    }

    public bool DevOpsLoginIsEnabled
    {
        get => _devOpsLoginIsEnabled;
        set => this.RaiseAndSetIfChanged(ref _devOpsLoginIsEnabled, value);
    }

    public Bitmap GitAvatarSource
    {
        get => _gitAvatarSource;
        set => this.RaiseAndSetIfChanged(ref _gitAvatarSource, value);
    }

    public bool GitAvatarIsVisible
    {
        get => _gitAvatarIsVisible;
        set => this.RaiseAndSetIfChanged(ref _gitAvatarIsVisible, value);
    }

    public bool GitLoginIsVisible
    {
        get => _gitLoginIsVisible;
        set => this.RaiseAndSetIfChanged(ref _gitLoginIsVisible, value);
    }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set => this.RaiseAndSetIfChanged(ref _isBusy, value);
    }

    public string BusyMessage
    {
        get => _busyMessage;
        set => this.RaiseAndSetIfChanged(ref _busyMessage, value);
    }

    public bool SettingsFlyoutIsOpen
    {
        get => _settingsFlyoutIsOpen;
        set => this.RaiseAndSetIfChanged(ref _settingsFlyoutIsOpen, value);
    }

    #endregion Properties

    #region Commands Decl

    public ICommand GitLoginCommand { get; }
    public ICommand DevOpsLoginCommand { get; }
    public ICommand RunMigrationCommand { get; }
    public ICommand BrowseTempPathCommand { get; }
    public ICommand BrowseGitPathCommand { get; }
    public ICommand BrowseLoggingPathCommand { get; }
    public ICommand CheckAllReposCommand { get; }
    public ICommand UpdateCheckedReposCountCommand { get; }
    public ICommand AboutWindowCommand { get; }
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
                if (avatarBytes != null && avatarBytes.Length > 0)
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
                // Show message box if needed, or update text
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

    private async Task BrowseGitPathAsync()
    {
        var window = _mainWindowLifetime.Value;
        if (window is not null)
        {
            var topLevel = TopLevel.GetTopLevel(window);
            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Git Bin Directory",
                AllowMultiple = false
            });
            if (folders.Count > 0)
            {
                var path = folders[0].Path.LocalPath;
                if (System.IO.File.Exists(Path.Combine(path, "git.exe")))
                {
                    GitBinPath = path;
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
