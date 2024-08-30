using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Web;
namespace SpotifyUser{
    public interface IUser
    {
        /// <summary>
        /// Get the login URI
        /// </summary>
        /// <returns></returns>
        string LoginURI { get; }

        /// <summary>
        /// Initiate the Spotify login process
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task InitiateSpotifyLoginAsync(HttpContext context);

        /// <summary>
        /// Get the Spotify client
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<SpotifyClient> GetSpotifyClientAsync(HttpContext context);
    }
}