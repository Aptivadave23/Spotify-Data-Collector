using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Spotify_Data_Collector;
using SpotifyAPI.Web;
namespace SpotifyUser{
    public interface IUser
    {
        /// <summary>
        /// Get the login URI
        /// </summary>
        /// <returns></returns>
        string LoginURI { get; }

        string SpotifyUserID { get; set;}

        SpotifyClient SpotifyClient { get; set; }
        /// <summary>
        /// Initiate the Spotify login process
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task InitiateSpotifyLoginAsync(HttpContext context);
        bool IsTokenExpired();
        /// <summary>
        /// Get the Spotify client
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<SpotifyClient> GetSpotifyClientAsync(HttpContext context);

        Task<List<TrackDTO>> GetRecentTracksAsync(SpotifyClient spotify, DateTimeOffset? startTime = null, DateTimeOffset? endTime = null);
    }
}