using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.Git
{
    /// <summary>
    ///     Class that returns a list of GitRepositories via github API.
    /// </summary>
    public class GetGitRepositories : IGitRepositories
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        /// <exception cref="ArgumentNullException"><paramref name="applicationSettings" /> is <see langword="null" />.</exception>
        public GetGitRepositories(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        /// <exception cref="InvalidOperationException">Accessing repos throws an error.</exception>
        public List<GitRepository> Value
        {
            get
            {
                try
                {
                    var api = "https://api.github.com";
                    var url = $"{api}/{_applicationSettings.GitSourceType}/{_applicationSettings.GitSource}/repos";
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("authorization",
                        $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}"))}");

                    var response = client.Execute<List<GitRepository>>(request);
                    var gitRepositories = response.Data;


                    if (response.Headers.Any(header => header.Name == "Link"))
                    {
                        var currentPage = 1;
                        var pageCount =
                            int.Parse(Regex.Match(response.Headers.First(header => header.Name.Equals("Link")).Value.ToString(), "page=([0-9]+)>; rel=\"last\"").Groups[1].Value);

                        while (currentPage < pageCount)
                        {
                            client.BaseUrl = new Uri($"{api}/{_applicationSettings.GitSourceType}/{_applicationSettings.GitSource}/repos?page={++currentPage}");
                            response = client.Execute<List<GitRepository>>(request);
                            gitRepositories.AddRange(response.Data);
                        }
                    }

                    gitRepositories = gitRepositories.OrderBy(repo => repo.Name).ToList();

                    return gitRepositories;
                }
                // ReSharper disable once CatchAllClause
                catch (Exception exception)
                {
                    throw new InvalidOperationException(exception.Message, exception);
                }
            }
        }
    }
}