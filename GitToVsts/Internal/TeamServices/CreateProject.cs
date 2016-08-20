using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates project through visualstudio.com api.
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
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            if (vsTsProcessTemplate == null)
            {
                throw new ArgumentNullException(nameof(vsTsProcessTemplate));
            }
            _applicationSettings = applicationSettings;
            _repository = repository;
            _vsTsProcessTemplate = vsTsProcessTemplate;
        }

        /// <summary>
        ///     Value.
        /// </summary>
        public VsTsCreateResponse Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}.visualstudio.com/defaultcollection/_apis/projects?api-version=2.0-preview");
                var request = new RestRequest(Method.POST);

                var json = new StringBuilder();
                json.Append($@"{{  ""name"": ""{_repository.Name}"",  ""description"": ""{_repository.Description}"",  ");
                json.Append(@"""capabilities"": {    ""versioncontrol"": {      ""sourceControlType"": ""Git""    },    ");
                json.Append($@"""processTemplate"": {{      ""templateTypeId"": ""{_vsTsProcessTemplate.Id}""      }}}}");

                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("projecttocreate", $@"""{_repository.Name}""");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.VsUser}:{_applicationSettings.VsPassword}"))}");
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

                var responseItem = client.Execute<VsTsCreateResponse>(request).Data;
                return responseItem;
            }
        }
    }
}