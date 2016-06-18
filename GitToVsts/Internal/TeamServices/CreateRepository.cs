using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    public class CreateRepository : ICreateRepository
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly Project _project;
        private readonly string _name;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CreateRepository(IApplicationSettings applicationSettings, Project project, string name)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            _applicationSettings = applicationSettings;
            _project = project;
            _name = name;
        }

        public VsTsRepository Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}.visualstudio.com/DefaultCollection/_apis/git/repositories/?api-version=1");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("gitrepositorytocreate", $@"""{_name}""");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.VsUser}:{_applicationSettings.VsPassword}"))}");
                request.AddParameter("application/json", $@"{{  ""name"": ""{_name}"",  ""project"": {{    ""id"": ""{_project.Id}""  }}}}", ParameterType.RequestBody);

                //var restResponse = client.ExecuteTask(request);

                //var response = client.Execute(request);

                //if (response.ResponseStatus == ResponseStatus.Completed)
                //{
                //    response.
                //}

                VsTsRepository repository = null;
                var asyncHandle = client.ExecuteAsync<VsTsRepository>(request, response => { repository = response.Data; });

                asyncHandle.Abort();

                //var repository = JsonConvert.DeserializeObject<VsTsRepository>(restResponse.Result.Content);
                return repository;
            }
        }
    }
}