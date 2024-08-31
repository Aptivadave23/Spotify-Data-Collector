using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity.Data;
using SpotifyDataCollector;
using SpotifyAPI.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Spotify_Data_Collector;

namespace SpotifyUser{
    
    public class User: IUser{
        private string _loginURI = "http://localhost:5272/redirect";
        private string _SpotifyUserID;
        private ISpotifyService _spotifyService = new Spotify();
        
        public string LoginURI{
            get{
                return _loginURI;
            }
        }

        public string SpotifyUserID{
            get{
                return _SpotifyUserID;
            }
            set{
                _SpotifyUserID = value;
            }
        }


        public Task InitiateSpotifyLoginAsync(HttpContext context)
        {
            var loginRequest = new SpotifyAPI.Web.LoginRequest(
                new Uri(_loginURI),
                _spotifyService.GetClientId(),
                SpotifyAPI.Web.LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserLibraryRead, Scopes.UserReadRecentlyPlayed, Scopes.UserTopRead }
            };
            var uri = loginRequest.ToUri();
            context.Response.Redirect(uri.ToString());
            return Task.CompletedTask;
        }

        public async Task<SpotifyClient> GetSpotifyClientAsync(HttpContext context)
        {
            var code = context.Request.Query["code"].ToString();
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(
                    _spotifyService.GetClientId(), // Replace with your Spotify Client ID
                    _spotifyService.GetClientSecret(), // Replace with your Spotify Client Secret
                    code,
                    new Uri(_loginURI)
                )
            );
            var spotify = new SpotifyClient(response.AccessToken);
            return spotify;
        }

        public async Task<List<TrackDTO>> GetRecentTracksAsync(SpotifyClient spotify, DateTimeOffset? startTime = null, DateTimeOffset? endTime = null)
        {
            var recentlyPlayedRequest = new PlayerRecentlyPlayedRequest()
            {
                Limit = 10 // Replace with the desired number of recent tracks to retrieve
            };
             // Convert startTime and endTime to milliseconds since Unix epoch if they are not null
            if (startTime.HasValue)
            {
                recentlyPlayedRequest.After = (long)(startTime.Value - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMilliseconds;
            }
            if (endTime.HasValue)
            {
                recentlyPlayedRequest.Before = (long)(endTime.Value - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMilliseconds;
            }
            var recentlyPlayed = await spotify.Player.GetRecentlyPlayed(recentlyPlayedRequest);
            var tracks = recentlyPlayed.Items.Select(item => new TrackDTO(
                item.Track.Name,
                item.Track.Id,
                item.Track.DurationMs.ToString(),
                item.Track.Popularity.ToString(),
                item.Track.ExternalUrls["spotify"],
                item.Track.Album.Id,
                item.Track.Album.ReleaseDate,
                item.Track.DiscNumber.ToString(),
                item.Track.TrackNumber.ToString()
            )).ToList();
            return tracks;
        }
        
        
    }
}