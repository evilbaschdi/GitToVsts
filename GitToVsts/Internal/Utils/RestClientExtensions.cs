using System;
using System.Threading.Tasks;
using RestSharp;

namespace GitToVsts.Internal.Utils
{
    /// <summary>
    ///     Extensions for RestClient.
    /// </summary>
    public static class RestClientExtensions
    {
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="restClient" /> is <see langword="null" />.
        ///     <paramref name="restRequest" /> is <see langword="null" />.
        /// </exception>
        public static async Task<IRestResponse> ExecuteTaskAsync(this IRestClient restClient, RestRequest restRequest)
        {
            if (restClient == null)
            {
                throw new ArgumentNullException(nameof(restClient));
            }
            if (restRequest == null)
            {
                throw new ArgumentNullException(nameof(restRequest));
            }
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