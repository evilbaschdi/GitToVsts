using System.Net;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Creates repository through visualstudio.com api.
/// </summary>
public class CreateRepository : ICreateRepository
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly string _name;
    private readonly VsTsProject _vsTsProject;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    /// <param name="vsTsProject"></param>
    /// <param name="name"></param>
    public CreateRepository(IApplicationSettings applicationSettings, VsTsProject vsTsProject, string name)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        _vsTsProject = vsTsProject ?? throw new ArgumentNullException(nameof(vsTsProject));
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    ///     Contains a VsTs Repository.
    /// </summary>
    public VsTsRepository Value
    {
        get
        {
            var client = new RestClient($"https://{_applicationSettings.VsSource}/DefaultCollection/_apis/git/repositories/?api-version=1.0");
            var request = new RestRequest
                          {
                              Method = Method.Post
                          };
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            // ReSharper disable once StringLiteralTypo
            request.AddHeader("gitrepositorytocreate", $@"""{_name}""");
            var username = !string.IsNullOrWhiteSpace(_applicationSettings.VsUser) ? _applicationSettings.VsUser + ":" : string.Empty;
            ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
            request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{_applicationSettings.VsPassword}"))}");
            request.AddParameter("application/json", $@"{{  ""name"": ""{_name}"",  ""project"": {{    ""id"": ""{_vsTsProject.Id}""  }}}}", ParameterType.RequestBody);

            var responseItem = client.ExecuteAsync<VsTsRepository>(request).Result.Data;
            return responseItem;
        }
    }
}