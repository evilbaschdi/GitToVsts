using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Class for requesting vsts process templates.
/// </summary>
public class GetTemplates : ITemplates
{
    private readonly IApplicationSettings _applicationSettings;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    public GetTemplates(IApplicationSettings applicationSettings)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
    }

    /// <summary>
    ///     Requested vsts process templates.
    /// </summary>
    public VsTsProcessTemplates Value
    {
        get
        {
            var client = new RestClient($"https://{_applicationSettings.VsSource}/DefaultCollection/_apis/process/processes?api-version=1.0");
            var request = new RestRequest
                          {
                              Method = Method.Get
                          };
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            var username = !string.IsNullOrWhiteSpace(_applicationSettings.VsUser) ? _applicationSettings.VsUser + ":" : string.Empty;
            request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{_applicationSettings.VsPassword}"))}");

            var processTemplates = client.ExecuteAsync<VsTsProcessTemplates>(request).Result.Data;
            return processTemplates;
        }
    }
}