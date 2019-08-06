using System;
using System.Net;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    /// <summary>
    ///     Class for requesting visualstudio team services projects.
    /// </summary>
    public class GetProjects : IProjects
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="applicationSettings"></param>
        public GetProjects(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        }

        /// <summary>
        ///     VsTs projects
        /// </summary>
        public VsTsProjects Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}/DefaultCollection/_apis/projects?api-version=2.0");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");

                var username = !string.IsNullOrWhiteSpace(_applicationSettings.VsUser) ? _applicationSettings.VsUser + ":" : string.Empty;
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{_applicationSettings.VsPassword}"))}");

                var projects = client.Execute<VsTsProjects>(request).Data;
                return projects;
            }
        }
    }
}