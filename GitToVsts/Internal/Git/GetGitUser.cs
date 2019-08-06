using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Class that requests an user from github API.
    /// </summary>
    public class GetGitUser : IGitUser
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="applicationSettings"></param>
        public GetGitUser(IApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        }

        /// <summary>
        ///     Contains a GitUser
        /// </summary>
        public GitUser Value
        {
            get
            {
                var client = new RestClient("https://api.github.com/user");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization",
                    $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}"))}");

                var gitUser = client.Execute<GitUser>(request).Data;
                return gitUser;
            }
        }
    }
}