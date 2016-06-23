using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    public class GetRepositories : IRepositories
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public GetRepositories(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public VsTsRepositories Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}.visualstudio.com/DefaultCollection/_apis/git/repositories?api-version=1.0");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.VsUser}:{_applicationSettings.VsPassword}"))}");

                var repositories = client.Execute<VsTsRepositories>(request).Data;
                return repositories;
            }
        }
    }
}