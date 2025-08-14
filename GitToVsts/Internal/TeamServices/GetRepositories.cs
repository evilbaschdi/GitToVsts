using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Class for requesting for Vsts Repositories.
/// </summary>
public class GetRepositories : IRepositories
{
    private readonly IApplicationSettings _applicationSettings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    public GetRepositories(IApplicationSettings applicationSettings)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
    }

    /// <summary>
    ///     Requested vsts repositories.
    /// </summary>
    public VsTsRepositories Value
    {
        get
        {
            var client = new RestClient($"https://{_applicationSettings.VsSource}/DefaultCollection/_apis/git/repositories?api-version=1.0");
            var request = new RestRequest
                          {
                              Method = Method.Get
                          };
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            var username = !string.IsNullOrWhiteSpace(_applicationSettings.VsUser) ? _applicationSettings.VsUser + ":" : string.Empty;
            request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{_applicationSettings.VsPassword}"))}");

            var repositories = client.ExecuteAsync<VsTsRepositories>(request).Result.Data;
            return repositories;
        }
    }
}