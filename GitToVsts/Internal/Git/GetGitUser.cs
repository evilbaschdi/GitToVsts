using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using Newtonsoft.Json;
using RestSharp;

namespace GitToVsts.Internal.Git
{
    public class GetGitUser : IGitUser
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public GetGitUser(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        public GitUser Value
        {
            get
            {
                var client = new RestClient("https://api.github.com/user");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}"))}");
                var response = client.Execute(request);
                var gitUser = JsonConvert.DeserializeObject<GitUser>(response.Content);
                return gitUser;
            }
        }
    }
}