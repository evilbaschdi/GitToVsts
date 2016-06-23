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
        private readonly VsTsProject _vsTsProject;
        private readonly string _name;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CreateRepository(IApplicationSettings applicationSettings, VsTsProject vsTsProject, string name)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            if (vsTsProject == null)
            {
                throw new ArgumentNullException(nameof(vsTsProject));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            _applicationSettings = applicationSettings;
            _vsTsProject = vsTsProject;
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
                request.AddParameter("application/json", $@"{{  ""name"": ""{_name}"",  ""project"": {{    ""id"": ""{_vsTsProject.Id}""  }}}}", ParameterType.RequestBody);

                var responseItem = client.Execute<VsTsRepository>(request).Data;
                return responseItem;
            }
        }
    }
}