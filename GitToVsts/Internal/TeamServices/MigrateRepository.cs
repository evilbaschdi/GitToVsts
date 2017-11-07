using System;
using System.IO;
using System.Linq;
using EvilBaschdi.Core.DirectoryExtensions;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Model;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Migrates a github repository to visualstudio team services.
    /// </summary>
    public class MigrateRepository : IMigrateRepository
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly ITemplates _templates;
        private readonly IProjects _projects;
        private readonly IGitCommands _gitCommands;
        private readonly string _template;
        private readonly string _project;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="applicationSettings" /> is <see langword="null" />.
        ///     <paramref name="templates" /> is <see langword="null" />.
        ///     <paramref name="projects" /> is <see langword="null" />.
        ///     <paramref name="gitCommands" /> is <see langword="null" />.
        ///     <paramref name="template" /> is <see langword="null" />.
        ///     <paramref name="project" /> is <see langword="null" />.
        /// </exception>
        public MigrateRepository(IApplicationSettings applicationSettings, ITemplates templates, IProjects projects, IGitCommands gitCommands, string template, string project)
        {
            _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
            _templates = templates ?? throw new ArgumentNullException(nameof(templates));
            _projects = projects ?? throw new ArgumentNullException(nameof(projects));
            _gitCommands = gitCommands ?? throw new ArgumentNullException(nameof(gitCommands));
            _template = template ?? throw new ArgumentNullException(nameof(template));
            _project = project ?? throw new ArgumentNullException(nameof(project));
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
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            try
            {
                var workingDir = $@"{_applicationSettings.TempPath}\{repository.Name}";
                Directory.CreateDirectory(workingDir);
                var cloneDir = $@"{workingDir}\{repository.Name}.git";

                var gitInfo = new GetGitProcessInfo(_applicationSettings);

                var getGitProcess = new GetGitProcess(gitInfo);
                //clone --mirror
                getGitProcess.Run(
                    $"{_gitCommands.Clone} {repository.Clone_Url.Replace("https://", $"https://{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}@")}", workingDir);

                var dirInfo = new DirectoryInfo(cloneDir);
                dirInfo.RenameTo(".git");
                //config --local --bool core.bare false
                getGitProcess.Run(_gitCommands.Config, workingDir);

                VsTsProject vsTsProject;
                if (_project.Contains("(default)"))
                {
                    var createProject = new CreateProject(_applicationSettings, repository, _templates.Value.Value.First(item => item.Name == _template));
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
                    vsTsProject = _projects.Value.Value.First(item => item.Name == _project);
                }

                var createRespository = new CreateRepository(_applicationSettings, vsTsProject, repository.Name);
                Console.Write(createRespository.Value.Id);

                var repositories = new GetRepositories(_applicationSettings).Value.Value;

                while (
                    !repositories.Any(
                        item =>
                            string.Equals(item.Name.Trim(), repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(item.Project.Id.Trim(), vsTsProject.Id.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    repositories = new GetRepositories(_applicationSettings).Value.Value;
                }

                var currentRepository =
                    repositories.First(
                        item =>
                            string.Equals(item.Name.Trim(), repository.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                            string.Equals(item.Project.Id.Trim(), vsTsProject.Id.Trim(), StringComparison.CurrentCultureIgnoreCase));

                //add remote vsts {currentRepository.RemoteUrl}
                getGitProcess.Run($"{_gitCommands.RemoteAdd} {currentRepository.RemoteUrl}", workingDir);

                //push --all vsts
                getGitProcess.Run(_gitCommands.PushAll, workingDir);

                //push --tags vsts
                getGitProcess.Run(_gitCommands.PushTags, workingDir);

                return new Response<string>
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
}