using System.Threading.Tasks;
using RestSharp;

namespace GitToVsts.Internal.Utils
{
    public static class RestClientExtensions
    {
        public static async Task<IRestResponse> ExecuteTask(this IRestClient restClient, RestRequest restRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            restClient.ExecuteAsync(restRequest, (restResponse, asyncHandle) =>
                                                 {
                                                     if (restResponse.ResponseStatus == ResponseStatus.Error)
                                                     {
                                                         tcs.SetException(restResponse.ErrorException);
                                                     }
                                                     else
                                                     {
                                                         tcs.SetResult(restResponse);
                                                     }
                                                 });
            return await tcs.Task;
        }
    }
}