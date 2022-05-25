using System.Threading.Tasks;

namespace MA.BlazorRest.Src.Contracts
{
    /// <summary>
    /// Jwt Servie For  Auto Add Authoriztion Header or auto Refresh Token Silently
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtToken"></param>
        /// <returns></returns>
        ValueTask SetTokenAsync(string? jwtToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ValueTask<string?> GetTokenAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        ValueTask SetRefereshTokenAsync(string? refreshToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ValueTask<string?> GetRefereshTokenAsync();
    }
}
