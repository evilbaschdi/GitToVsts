using System;
using System.Collections.Generic;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using Newtonsoft.Json;
using RestSharp;

namespace GitToVsts.Internal.Git
{
    public class GetGitRepositories : IGitRepositories
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public GetGitRepositories(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        public List<GitRepository> Value
        {
            get
            {
                var api = "https://api.github.com";
                //https://api.github.com/orgs/globalconcepts/repos
                var url = $"{api}/{_applicationSettings.GitSourceType}/{_applicationSettings.GitSource}/repos";
                var client = new RestClient(url);

                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}"))}");

                var response = client.Execute(request);
                var gitRepositories = JsonConvert.DeserializeObject<List<GitRepository>>(response.Content);
                return gitRepositories;
            }
        }
    }
}