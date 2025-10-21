using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Creates repository through Azure DevOps API.
/// </summary>
public class CreateRepository : ICreateRepository
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly string _name;
    private readonly DevOpsProject _devOpsProject;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    /// <param name="devOpsProject"></param>
    /// <param name="name"></param>
    public CreateRepository(IApplicationSettings applicationSettings, DevOpsProject devOpsProject, string name)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        _devOpsProject = devOpsProject ?? throw new ArgumentNullException(nameof(devOpsProject));
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    ///     Contains a DevOps Repository.
    /// </summary>
    public DevOpsRepository Value
    {
        get
        {
            var client = new RestClient($"https://{_applicationSettings.DevOpsSource}/DefaultCollection/_apis/git/repositories/?api-version=7.1");
            var request = new RestRequest
                          {
                              Method = Method.Post
                          };
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            // ReSharper disable once StringLiteralTypo
            request.AddHeader("gitrepositorytocreate", $@"""{_name}""");
            var username = !string.IsNullOrWhiteSpace(_applicationSettings.DevOpsUser) ? Uri.EscapeDataString(_applicationSettings.DevOpsUser) : "pat";
            var devOpsToken = Uri.EscapeDataString(_applicationSettings.DevOpsPersonalAccessToken);
            request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{devOpsToken}"))}");
            request.AddParameter("application/json", $@"{{  ""name"": ""{_name}"",  ""project"": {{    ""id"": ""{_devOpsProject.Id}""  }}}}", ParameterType.RequestBody);

            var responseItem = client.ExecuteAsync<DevOpsRepository>(request).Result.Data;
            return responseItem;
        }
    }
}

