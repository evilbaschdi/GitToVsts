using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EvilBaschdi.Core.Application;
using EvilBaschdi.Core.Browsers;
using EvilBaschdi.Core.DirectoryExtensions;
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
    public partial class MainWindow : MetroWindow
    {
        public ObservableCollection<GitRepositoryObservableCollectionItem> GitRepositoryObservableCollection { get; set; }
        private IGitRepositories _gitRepositories;
        private readonly IMetroStyle _style;
        private readonly IApplicationSettings _applicationSettings;
        private int _overrideProtection;
        private string _loggingPath;
        private IProjects _projects;
        private ITemplates _templates;

        public MainWindow()
        {
            ISettings coreSettings = new CoreSettings();
            _applicationSettings = new ApplicationSettings();

            InitializeComponent();
            _style = new MetroStyleByToggleSwitch(this, Accent, ThemeSwitch, coreSettings);
            _style.Load(true, false);
            Load();
        }

        #region GitTab

        private void LoadGitRepositoryList()
        {
            GitRepositoryObservableCollection = GetGitRepositoryObservableCollection();
            GitRepositoryObservableCollectionBox.ItemsSource = GitRepositoryObservableCollection;
        }

        private ObservableCollection<GitRepositoryObservableCollectionItem> GetGitRepositoryObservableCollection()
        {
            var collection = new ObservableCollection<GitRepositoryObservableCollectionItem>();

            foreach (var repository in _gitRepositories.Value)
            {
                collection.Add(new GitRepositoryObservableCollectionItem
                               {
                                   Displayname = repository.Name,
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
                SetPicture(getGitUser.Value);
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

        private void SetPicture(GitUser gitUser)
        {
            var image = new BitmapImage();
            int BytesToRead = 100;

            if (!string.IsNullOrWhiteSpace(gitUser.Avatar_Url))
            {
                var pictureUri = new Uri(gitUser.Avatar_Url, UriKind.Absolute);
                WebRequest request = WebRequest.Create(pictureUri);
                request.Timeout = -1;
                WebResponse response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                BinaryReader reader = new BinaryReader(responseStream);
                MemoryStream memoryStream = new MemoryStream();

                byte[] bytebuffer = new byte[BytesToRead];
                int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

                while (bytesRead > 0)
                {
                    memoryStream.Write(bytebuffer, 0, bytesRead);
                    bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
                }

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);

                image.StreamSource = memoryStream;
                image.EndInit();

                GitAvatar.Source = image;
                GitAvatar.Visibility = Visibility.Visible;
                GitLogin.Visibility = Visibility.Hidden;
            }
        }

        #endregion GitTab

        #region RepoTab

        private void MigrateToVsTsOnClick(object sender, RoutedEventArgs e)
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

        #endregion VsTsTab

        #region RunTab

        private void RunMigrationOnClick(object sender, RoutedEventArgs e)
        {
            Run();
        }

        private void Run()
        {
            var checkedItems = GitRepositoryObservableCollection.Where(attribute => attribute.MigrateToVsTs);

            foreach (var checkedItem in checkedItems)
            {
                var workingDir = $@"{_applicationSettings.TempPath}\{checkedItem.Repository.Name}";
                Directory.CreateDirectory(workingDir);
                var cloneDir = $@"{workingDir}\{checkedItem.Repository.Name}.git";
                var commands = new GitCommands();
                var gitInfo = new GetGitProcessInfo(_applicationSettings);

                var clone = new GetGitProcess(gitInfo);
                clone.Run($"{commands.Clone} {checkedItem.Repository.Clone_Url}", workingDir);

                var dirInfo = new DirectoryInfo(cloneDir);
                dirInfo.RenameTo(".git");

                var config = new GetGitProcess(gitInfo);
                config.Run(commands.Config, workingDir);

                var reset = new GetGitProcess(gitInfo);
                reset.Run(commands.Reset, workingDir);

                VsTsProject vsTsProject;
                if (VsProjects.Text.Contains("(default)"))
                {
                    var createProject = new CreateProject(_applicationSettings, checkedItem.Repository, _templates.Value.Value.First(item => item.Name == VsTemplates.Text));
                    Console.Write(createProject.Value.Id);

                    var projects = new GetProjects(_applicationSettings).Value.Value;

                    while (!projects.Any(item => string.Equals(item.Name.Trim(), checkedItem.Repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                    {
                        projects = new GetProjects(_applicationSettings).Value.Value;
                    }


                    vsTsProject = projects.First(item => string.Equals(item.Name.Trim(), checkedItem.Repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    vsTsProject = _projects.Value.Value.First(item => item.Name == VsProjects.Text);
                }

                var createRespository = new CreateRepository(_applicationSettings, vsTsProject, checkedItem.Repository.Name);
                Console.Write(createRespository.Value.Id);

                var repositories = new GetRepositories(_applicationSettings).Value.Value;

                while (
                    !repositories.Any(
                        item =>
                            string.Equals(item.Name.Trim(), checkedItem.Repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(item.Project.Id.Trim(), vsTsProject.Id.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    repositories = new GetRepositories(_applicationSettings).Value.Value;
                }

                var currentRepository =
                    repositories.First(
                        item =>
                            string.Equals(item.Name.Trim(), checkedItem.Repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(item.Project.Id.Trim(), vsTsProject.Id.Trim(), StringComparison.CurrentCultureIgnoreCase));

                var addRemote = new GetGitProcess(gitInfo);
                addRemote.Run($"{commands.RemoteAdd} {currentRepository.RemoteUrl}", workingDir);

                var pushAll = new GetGitProcess(gitInfo);
                pushAll.Run(commands.PushAll, workingDir);

                var pushTags = new GetGitProcess(gitInfo);
                pushTags.Run(commands.PushTags, workingDir);


                ShowMessage("Finished", "All repositories where migrated.");
            }
        }

        #endregion RunTab

        #region Window Methods

        private void Load()
        {
            LoggingPath.Text = _applicationSettings.LoggingPath;
            TempPath.Text = _applicationSettings.TempPath;
            GitBinPath.Text = _applicationSettings.GitBinPath;
            GitUsername.Text = _applicationSettings.GitUser;
            GitPassword.Password = _applicationSettings.GitPassword;
            GitSource.Text = _applicationSettings.GitSource;
            VsUsername.Text = _applicationSettings.VsUser;
            VsPassword.Password = _applicationSettings.VsPassword;
            VsSource.Text = _applicationSettings.VsSource;
            VsProjects.Text = _applicationSettings.VsProject;

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
            var browser = new ExplorerFolderBrower
                          {
                              SelectedPath = _applicationSettings.LoggingPath
                          };
            browser.ShowDialog();
            _applicationSettings.LoggingPath = browser.SelectedPath;
            Load();
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(LoggingPath.Text))
            {
                _applicationSettings.LoggingPath = LoggingPath.Text;
                Load();
            }
        }

        private void BrowseTempPathClick(object sender, RoutedEventArgs e)
        {
            var browser = new ExplorerFolderBrower
                          {
                              SelectedPath = _applicationSettings.TempPath
                          };
            browser.ShowDialog();
            _applicationSettings.TempPath = browser.SelectedPath;
            Load();
        }

        private void TempPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(TempPath.Text))
            {
                _applicationSettings.TempPath = TempPath.Text;
                Load();
            }
        }

        private void BrowseGitPathClick(object sender, RoutedEventArgs e)
        {
            var browser = new ExplorerFolderBrower
                          {
                              SelectedPath = _applicationSettings.GitBinPath
                          };
            browser.ShowDialog();
            if (File.Exists(browser.SelectedPath + "\\git.exe"))
            {
                _applicationSettings.GitBinPath = browser.SelectedPath;
                Load();
            }
            else
            {
                ShowMessage("Path Error", "Path does not contain a 'git.exe'");
            }
        }

        private void GitPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(GitBinPath.Text) && File.Exists(GitBinPath.Text + "\\git.exe"))
            {
                _applicationSettings.GitBinPath = GitBinPath.Text;
                Load();
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
            _style.SetTheme(sender);
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