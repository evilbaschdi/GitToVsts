using System;
using System.IO;
using System.Linq;
using EvilBaschdi.Core.DirectoryExtensions;
using GitToVsts.Core;
using GitToVsts.Internal.Git;
using GitToVsts.Internal.Models;

namespace GitToVsts.Internal.TeamServices
{
    public class MigrateRepository : IMigrateRepository
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly ITemplates _templates;
        private readonly IProjects _projects;
        private readonly IGitCommands _gitCommands;
        private readonly string _template;
        private readonly string _project;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public MigrateRepository(IApplicationSettings applicationSettings, ITemplates templates, IProjects projects, IGitCommands gitCommands, string template, string project)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            if (templates == null)
            {
                throw new ArgumentNullException(nameof(templates));
            }
            if (projects == null)
            {
                throw new ArgumentNullException(nameof(projects));
            }
            if (gitCommands == null)
            {
                throw new ArgumentNullException(nameof(gitCommands));
            }
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            _applicationSettings = applicationSettings;
            _templates = templates;
            _projects = projects;
            _gitCommands = gitCommands;
            _template = template;
            _project = project;
        }

        public int For(GitRepository repository)
        {
            var workingDir = $@"{_applicationSettings.TempPath}\{repository.Name}";
            Directory.CreateDirectory(workingDir);
            var cloneDir = $@"{workingDir}\{repository.Name}.git";

            var gitInfo = new GetGitProcessInfo(_applicationSettings);

            var getGitProcess = new GetGitProcess(gitInfo);
            //clone --mirror
            getGitProcess.Run($"{_gitCommands.Clone} {repository.Clone_Url}", workingDir);

            var dirInfo = new DirectoryInfo(cloneDir);
            dirInfo.RenameTo(".git");
            //config --local --bool core.bare false
            getGitProcess.Run(_gitCommands.Config, workingDir);
            //reset --hard HEAD
            getGitProcess.Run(_gitCommands.Reset, workingDir);

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

            return 200;
        }
    }
}