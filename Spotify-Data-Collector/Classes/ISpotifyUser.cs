using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Web;
using SpotifyDataCollector;

namespace SpotifyUser
{
    public interface IUser
    {
        // Properties
        string LoginURI { get; }
        string SpotifyUserID { get; set; }
        SpotifyClient SpotifyClient { get; set; }
        string SpotifyToken { get; set; }
        string TokenExpireTime { get; set; }
        string SpotifyAccessCode { get; set; }

        // Methods
        Task InitiateSpotifyLoginAsync(HttpContext context);
        Task<SpotifyClient> GetSpotifyClientAsync(string code);
        Task RefreshTokenAsync();
        bool IsTokenExpired();

        // Remove SpotifyClient parameter since it's already a property
        Task<List<UserTrackDTO>> GetRecentTracksAsync(DateTimeOffset? startTime = null, DateTimeOffset? endTime = null, int trackCount = 10);
    }
}
