using System;
using System.Text;
using GitToVsts.Core;
using GitToVsts.Internal.Models;
using RestSharp;

namespace GitToVsts.Internal.TeamServices
{
    public class GetTemplates : ITemplates
    {
        private readonly IApplicationSettings _applicationSettings;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public GetTemplates(IApplicationSettings applicationSettings)
        {
            if (applicationSettings == null)
            {
                throw new ArgumentNullException(nameof(applicationSettings));
            }
            _applicationSettings = applicationSettings;
        }

        public VsTsProcessTemplates Value
        {
            get
            {
                var client = new RestClient($"https://{_applicationSettings.VsSource}.visualstudio.com/DefaultCollection/_apis/process/processes?api-version=1");
                var request = new RestRequest(Method.GET);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_applicationSettings.VsUser}:{_applicationSettings.VsPassword}"))}");

                var processTemplates = client.Execute<VsTsProcessTemplates>(request).Data;
                return processTemplates;
            }
        }
    }
}