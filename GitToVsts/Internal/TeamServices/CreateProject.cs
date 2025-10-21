using System.Text;
using GitToVsts.Core;
using GitToVsts.Model;
using RestSharp;

namespace GitToVsts.Internal.TeamServices;

/// <summary>
///     Creates project through Azure DevOps API.
/// </summary>
public class CreateProject : ICreateProject
{
    private readonly IApplicationSettings _applicationSettings;
    private readonly GitRepository _repository;
    private readonly DevOpsProcessTemplate _devOpsProcessTemplate;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="applicationSettings"></param>
    /// <param name="repository"></param>
    /// <param name="devOpsProcessTemplate"></param>
    public CreateProject(IApplicationSettings applicationSettings, GitRepository repository, DevOpsProcessTemplate devOpsProcessTemplate)
    {
        _applicationSettings = applicationSettings ?? throw new ArgumentNullException(nameof(applicationSettings));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _devOpsProcessTemplate = devOpsProcessTemplate ?? throw new ArgumentNullException(nameof(devOpsProcessTemplate));
    }

    /// <summary>
    ///     Value.
    /// </summary>
    public DevOpsCreateResponse Value
    {
        get
        {
            // ReSharper disable once StringLiteralTypo
            var client = new RestClient($"https://{_applicationSettings.DevOpsSource}/defaultcollection/_apis/projects?api-version=7.1");
            var request = new RestRequest
                          {
                              Method = Method.Post
                          };

            var json = new StringBuilder();
            json.Append($@"{{  ""name"": ""{_repository.Name}"",  ""description"": ""{_repository.Description}"",  ");
            // ReSharper disable once StringLiteralTypo
            json.Append(@"""capabilities"": {    ""versioncontrol"": {      ""sourceControlType"": ""Git""    },    ");
            json.Append($@"""processTemplate"": {{      ""templateTypeId"": ""{_devOpsProcessTemplate.Id}""      }}}}");

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            // ReSharper disable once StringLiteralTypo
            request.AddHeader("projecttocreate", $@"""{_repository.Name}""");
            var username = !string.IsNullOrWhiteSpace(_applicationSettings.DevOpsUser) ? Uri.EscapeDataString(_applicationSettings.DevOpsUser) : "pat";
            var devOpsToken = Uri.EscapeDataString(_applicationSettings.DevOpsPersonalAccessToken);
            request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{devOpsToken}"))}");
            request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);

            var responseItem = client.ExecuteAsync<DevOpsCreateResponse>(request).Result.Data;
            return responseItem;
        }
    }
}

