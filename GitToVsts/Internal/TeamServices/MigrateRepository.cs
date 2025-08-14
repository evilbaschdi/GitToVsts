using System.IO;
using EvilBaschdi.Core.Extensions;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Model;
using JetBrains.Annotations;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Migrates a github repository to visualstudio team services.
/// </summary>
public class MigrateRepository : IMigrateRepository
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly IGitCommands _gitCommands;
    private readonly IMigrationConfiguration _migrationConfiguration;
    private readonly IProjects _projects;
    private readonly ITemplates _templates;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    /// <param name="templates"></param>
    /// <param name="projects"></param>
    /// <param name="gitCommands"></param>
    /// <param name="migrationConfiguration"></param>
    public MigrateRepository(IApplicationSettings applicationSettings, ITemplates templates, IProjects projects, IGitCommands gitCommands,
                             [NotNull] IMigrationConfiguration migrationConfiguration)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        _templates = templates ?? throw new ArgumentNullException(nameof(templates));
        _projects = projects ?? throw new ArgumentNullException(nameof(projects));
        _gitCommands = gitCommands ?? throw new ArgumentNullException(nameof(gitCommands));
        _migrationConfiguration = migrationConfiguration ?? throw new ArgumentNullException(nameof(migrationConfiguration));
    }

    /// <summary>
    ///     Contains the response of repository migration.
    /// </summary>
    /// <param name="repository">GitRepository to migrate.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"><paramref name="repository" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">somethings wrong.</exception>
    public Response<string> ValueFor(GitRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        try
        {
            var workingDir = $@"{_applicationSettings.TempPath}\{repository.Name}";
            Directory.CreateDirectory(workingDir);
            var cloneDir = $@"{workingDir}\{repository.Name}.git";

            var gitInfo = new GetGitProcessInfo(_applicationSettings);

            IGitProcess getGitProcess = new GetGitProcess(gitInfo);
            //clone --mirror
            getGitProcess.Run(
                $"{_gitCommands.Clone} {repository.Clone_Url.Replace("https://", $"https://{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}@")}", workingDir);

            var dirInfo = new DirectoryInfo(cloneDir);
            dirInfo.RenameTo(".git");
            //config --local --bool core.bare false
            getGitProcess.Run(_gitCommands.Config, workingDir);

            VsTsProject vsTsProject;
            if (_migrationConfiguration.VsProject.Contains("(default)"))
            {
                var createProject = new CreateProject(_applicationSettings, repository, _templates.Value.Value.First(item => item.Name == _migrationConfiguration.VsTemplate));
                Console.Write(createProject.Value.Id);

                var projects = new GetProjects(_applicationSettings).Value.Value;

                while (!projects.Any(item => string.Equals(item.Name.Trim(), repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    projects = new GetProjects(_applicationSettings).Value.Value;
                }

                vsTsProject = projects.First(item => string.Equals(item.Name.Trim(), repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }
            else
            {
                vsTsProject = _projects.Value.Value.First(item => item.Name == _migrationConfiguration.VsProject);
            }

            var createRepository = new CreateRepository(_applicationSettings, vsTsProject, repository.Name);
            Console.Write(createRepository.Value.Id);

            var repositories = new GetRepositories(_applicationSettings).Value.Value;

            while (
                !repositories.Any(item =>
                                      string.Equals(item.Name.Trim(), repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                                      string.Equals(item.Project.Id.Trim(), vsTsProject.Id.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                repositories = new GetRepositories(_applicationSettings).Value.Value;
            }

            var currentRepository =
                repositories.First(item =>
                                       string.Equals(item.Name.Trim(), repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                                       string.Equals(item.Project.Id.Trim(), vsTsProject.Id.Trim(), StringComparison.CurrentCultureIgnoreCase));

            //add remote vsts {currentRepository.RemoteUrl}
            getGitProcess.Run($"{_gitCommands.RemoteAdd} {currentRepository.RemoteUrl}", workingDir);

            //push --all vsts
            getGitProcess.Run(_gitCommands.PushAll, workingDir);

            //push --tags vsts
            getGitProcess.Run(_gitCommands.PushTags, workingDir);

            return new()
                   {
                       Code = 200,
                       Value = workingDir
                   };
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }
}