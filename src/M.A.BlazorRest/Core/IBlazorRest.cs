using MA.BlazorRest.Src.Responses;
using System.Threading;
using System.Threading.Tasks;
using MA.BlazorRest.M.A.BlazorRest.Responses;

namespace MA.BlazorRest.Src.Core
{
    public interface IBlazorRest
    {
        /// <summary>
        ///  send http request with basic response type
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BaseResponse> SendAsync(IBlazorRestMessage message, CancellationToken cancellationToken = default);

        /// <summary>
        ///  send http request with specific response type
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BaseResponse<TResponse>> SendAsync<TResponse>(IBlazorRestMessage message, CancellationToken cancellationToken = default);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="message"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BaseResponse<TResponse>> SendAsync<TResponse>(IBlazorRestMessage message, ResponseOptions responseOptions, CancellationToken cancellationToken = default);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BaseResponse<TResponse>> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default);


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="responseOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BaseResponse<TResponse>> GetAsync<TResponse>(string url, ResponseOptions responseOptions, CancellationToken cancellationToken = default);
    }
}
