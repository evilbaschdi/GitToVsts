using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using Newtonsoft.Json;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    public class CreateProject : IProject
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly GitRepository _repository;
        private readonly ProcessTemplate _processTemplate;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CreateProject(IApplicationSettings applicationSettings, GitRepository repository, ProcessTemplate processTemplate)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            if (processTemplate == null)
            {
                throw new ArgumentNullException(nameof(processTemplate));
            }
            _applicationSettings = applicationSettings;
            _repository = repository;
            _processTemplate = processTemplate;
        }

        public Response Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}.visualstudio.com/defaultcollection/_apis/projects?api-version=2.0-preview");
                var request = new RestRequest(Method.POST);

                var json = new StringBuilder();
                json.Append($@"{{  ""name"": ""{_repository.Name}"",  ""description"": ""{_repository.Description}"",  ");
                json.Append($@"""capabilities"": {{    ""versioncontrol"": {{      ""sourceControlType"": ""Git""    }},    ");
                json.Append($@"""processTemplate"": {{      ""templateTypeId"": ""{_processTemplate.Id}""      }}}}");

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("projecttocreate", $@"""{_repository.Name}""");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.VsUser}:{_applicationSettings.VsPassword}"))}");
                //request.AddParameter("application/json",
                //    $@"{{  ""name"": ""{_repository.Name}"",  ""description"": ""{_repository.Description}"",  ""capabilities"": {{    ""versioncontrol"": {{      ""sourceControlType"": ""Git""    }},    ""processTemplate"": {{      ""templateTypeId"": ""{_processTemplate
                //        .Id}""      }}}}", ParameterType.RequestBody);
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                var response = client.Execute(request);
                var responseItem = JsonConvert.DeserializeObject<Response>(response.Content);
                return responseItem;
            }
        }
    }
}