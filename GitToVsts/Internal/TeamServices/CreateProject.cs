using System;
using System.Net;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates project through visualstudio.com API.
    /// </summary>
    public class CreateProject : ICreateProject
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly GitRepository _repository;
        private readonly VsTsProcessTemplate _vsTsProcessTemplate;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="applicationSettings" /> is <see langword="null" />.
        ///     <paramref name="repository" /> is <see langword="null" />.
        ///     <paramref name="vsTsProcessTemplate" /> is <see langword="null" />.
        /// </exception>
        public CreateProject(IApplicationSettings applicationSettings, GitRepository repository, VsTsProcessTemplate vsTsProcessTemplate)
        {
            _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _vsTsProcessTemplate = vsTsProcessTemplate ?? throw new ArgumentNullException(nameof(vsTsProcessTemplate));
        }

        /// <summary>
        ///     Value.
        /// </summary>
        public VsTsCreateResponse Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}/defaultcollection/_apis/projects?api-version=2.0-preview");
                var request = new RestRequest(Method.POST);

                var json = new StringBuilder();
                json.Append($@"{{  ""name"": ""{_repository.Name}"",  ""description"": ""{_repository.Description}"",  ");
                json.Append(@"""capabilities"": {    ""versioncontrol"": {      ""sourceControlType"": ""Git""    },    ");
                json.Append($@"""processTemplate"": {{      ""templateTypeId"": ""{_vsTsProcessTemplate.Id}""      }}}}");

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("projecttocreate", $@"""{_repository.Name}""");
                var username = !string.IsNullOrWhiteSpace(_applicationSettings.VsUser) ? _applicationSettings.VsUser + ":" : string.Empty;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{_applicationSettings.VsPassword}"))}");
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                var responseItem = client.Execute<VsTsCreateResponse>(request).Data;
                return responseItem;
            }
        }
    }
}