using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

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
    ///     DevOps projects
    /// </summary>
        public DevOpsProjects Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.DevOpsSource}/DefaultCollection/_apis/projects");
                var request = new RestRequest
                              {
                                  Method = Method.Get
                              };
                request.AddHeader("cache-control", "no-cache");
    
                var username = !string.IsNullOrWhiteSpace(_applicationSettings.DevOpsUser) ? Uri.EscapeDataString(_applicationSettings.DevOpsUser) : "pat";
                var devOpsToken = Uri.EscapeDataString(_applicationSettings.DevOpsPersonalAccessToken);
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{devOpsToken}"))}");
    
                var projects = client.ExecuteAsync<DevOpsProjects>(request).Result.Data;
                return projects;
            }
        }
    }
    