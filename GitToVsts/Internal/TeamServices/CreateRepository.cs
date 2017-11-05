using System;
using System.Net;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Creates repository through visualstudio.com api.
    /// </summary>
    public class CreateRepository : ICreateRepository
    {
        private readonly IApplicationSettings _applicationSettings;
        private readonly VsTsProject _vsTsProject;
        private readonly string _name;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="applicationSettings" /> is <see langword="null" />.
        ///     <paramref name="vsTsProject" /> is <see langword="null" />.
        ///     <paramref name="name" /> is <see langword="null" />.
        /// </exception>
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

        /// <summary>
        ///     Contains a VsTs Repository.
        /// </summary>
        public VsTsRepository Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}/DefaultCollection/_apis/git/repositories/?api-version=1.0");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("gitrepositorytocreate", $@"""{_name}""");
                var username = !string.IsNullOrWhiteSpace(_applicationSettings.VsUser) ? _applicationSettings.VsUser + ":" : string.Empty;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{_applicationSettings.VsPassword}"))}");
                request.AddParameter("application/json", $@"{{  ""name"": ""{_name}"",  ""project"": {{    ""id"": ""{_vsTsProject.Id}""  }}}}", ParameterType.RequestBody);

                var responseItem = client.Execute<VsTsRepository>(request).Data;
                return responseItem;
            }
        }
    }
}