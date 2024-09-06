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
using System.Runtime.Serialization;

namespace SpotifyUser{
    
    public class User: IUser{
        private string _loginURI = 
            Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI");
        private string _SpotifyUserID;
        private ISpotifyService _spotifyService = new Spotify();
        private SpotifyClient _spotifyClient;
        private string _spotifyToken;
        private string _spotifyAccessCode;
        private string _tokenExpireTime;
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

        public string TokenExpireTime{
            get{
                return _tokenExpireTime;
            }
            set{
                _tokenExpireTime = value;
            }
        }

        public SpotifyClient SpotifyClient{
            get{
                return _spotifyClient;
            }
            set{
                _spotifyClient = value;
            }
        }

        public string SpotifyToken{
            get{
                return _spotifyToken;
            }
            set{
                _spotifyToken = value;
            }
        }

        public string SpotifyAccessCode{
            get{
                return _spotifyAccessCode;
            }
            set{
                _spotifyAccessCode = value;
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

        public async Task<SpotifyClient> GetSpotifyClientAsync(string code)
        {           
            SpotifyAccessCode = code;
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(
                    _spotifyService.GetClientId(), // Replace with your Spotify Client ID
                    _spotifyService.GetClientSecret(), // Replace with your Spotify Client Secret
                    SpotifyAccessCode,
                    new Uri(_loginURI)
                )
            );

            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new AuthorizationCodeAuthenticator(
                    _spotifyService.GetClientId(), // Replace with your Spotify Client ID
                    _spotifyService.GetClientSecret(), // Replace with your Spotify Client Secret
                    response
                )
            );
            SpotifyClient = new SpotifyClient(config);
            SpotifyToken = response.RefreshToken;            
            TokenExpireTime = DateTime.Now.AddSeconds(response.ExpiresIn).ToString();
            return SpotifyClient;
        
        }

        public async Task RefreshTokenAsync()
        {
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeRefreshRequest(
                    _spotifyService.GetClientId(), // Replace with your Spotify Client ID
                    _spotifyService.GetClientSecret(), // Replace with your Spotify Client Secret
                    _spotifyToken
                )
            );
            
            SpotifyClient = new SpotifyClient(response.AccessToken);
            SpotifyToken = response.RefreshToken;
            TokenExpireTime = DateTime.Now.AddSeconds(response.ExpiresIn).ToString();
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Retrieves a list of recent tracks from the Spotify API.
        /// </summary>
        /// <param name="spotify">The SpotifyClient instance used to make API requests.</param>
        /// <param name="startTime">Optional. The start time to filter the recent tracks. Defaults to null.</param>
        /// <param name="endTime">Optional. The end time to filter the recent tracks. Defaults to null.</param>
        /// <returns>A list of TrackDTO objects representing the recent tracks.</returns>
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
                item.Track.TrackNumber.ToString(),
                item.Track.Artists[0].Id,
                item.Track.Artists[0].Name
            )).ToList();
            return tracks;
        }
        
        /// <summary>
        /// Check if the token will expire in the next minute
        /// </summary>
        /// <returns>
        /// True if the token will expire in the next minute, false otherwise
        /// </returns>
        public bool IsTokenExpired()
        {
            return DateTime.Now > DateTime.Parse(TokenExpireTime) - TimeSpan.FromSeconds(60);
        }
        
    }
}