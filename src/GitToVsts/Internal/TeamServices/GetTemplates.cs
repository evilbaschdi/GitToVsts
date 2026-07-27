using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Class for requesting devops process templates.
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
///     Requested devops process templates.
/// </summary>
        public DevOpsProcessTemplates Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.DevOpsSource}/DefaultCollection/_apis/process/processes?api-version=7.1");
                var request = new RestRequest
                              {
                                  Method = Method.Get
                              };
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                var username = !string.IsNullOrWhiteSpace(_applicationSettings.DevOpsUser) ? Uri.EscapeDataString(_applicationSettings.DevOpsUser) : "pat";
                var devOpsToken = Uri.EscapeDataString(_applicationSettings.DevOpsPersonalAccessToken);
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{devOpsToken}"))}");
    
                var processTemplates = client.ExecuteAsync<DevOpsProcessTemplates>(request).Result.Data;
                return processTemplates;
            }
        }
    }
    

