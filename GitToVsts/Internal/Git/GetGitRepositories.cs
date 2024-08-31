using System.Text;
using System.Text.RegularExpressions;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.Git;

/// <summary>
///     Class that returns a list of GitRepositories via github API.
/// </summary>
public partial class GetGitRepositories : IGitRepositories
{
    private readonly IApplicationSettings _applicationSettings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    public GetGitRepositories(IApplicationSettings applicationSettings)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
    }

    /// <exception cref="InvalidOperationException">Accessing repos throws an error.</exception>
    public List<GitRepository> Value
    {
        get
        {
            try
            {
                const string api = "https://api.github.com";
                var url = $"{api}/{_applicationSettings.GitSourceType}/{_applicationSettings.GitSource}/repos";
                var client = new RestClient(url);
                var request = new RestRequest
                              {
                                  Method = Method.Get
                              };
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization",
                    $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.GitUser}:{_applicationSettings.GitPassword}"))}");

                var response = client.ExecuteAsync<List<GitRepository>>(request).Result;
                var gitRepositories = response.Data;

                if (response.Headers != null && response.Headers.Any(header => header.Name == "Link"))
                {
                    var currentPage = 1;
                    var value = response.Headers.First(header => header.Name is "Link").Value;
                    {
                        var pageCount =
                            int.Parse(LastLink().Match(value).Groups[1].Value);

                        while (currentPage < pageCount)
                        {
                            url = $"{api}/{_applicationSettings.GitSourceType}/{_applicationSettings.GitSource}/repos?page={++currentPage}";
                            client = new RestClient(url);
                            response = client.ExecuteAsync<List<GitRepository>>(request).Result;
                            if (gitRepositories != null && response.Data != null)
                            {
                                gitRepositories.AddRange(response.Data);
                            }
                        }
                    }
                }

                gitRepositories = gitRepositories?.OrderBy(repo => repo.Name).ToList();

                return gitRepositories;
            }
            // ReSharper disable once CatchAllClause
            catch (Exception exception)
            {
                throw new InvalidOperationException(exception.Message, exception);
            }
        }
    }

    [GeneratedRegex("page=([0-9]+)>{} rel=\"last\"")]
    private static partial Regex LastLink();
}