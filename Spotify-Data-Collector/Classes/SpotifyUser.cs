using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Web;
using SpotifyDataCollector;
using Spotify_Data_Collector;

namespace SpotifyUser
{
    public class User : IUser
    {
        private readonly ISpotifyService _spotifyService;
        private readonly string _loginURI;
        private string _spotifyUserID;
        private SpotifyClient _spotifyClient;
        private string _spotifyToken;
        private string _spotifyAccessCode;
        private string _tokenExpireTime;

        // Constructor to initialize the User with injected dependencies
        public User(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService ?? throw new ArgumentNullException(nameof(spotifyService));
            _loginURI = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new ArgumentNullException("SPOTIFY_REDIRECT_URI not found");
            _spotifyAccessCode = Environment.GetEnvironmentVariable("SPOTIFY_CODE") ?? throw new ArgumentNullException("SPOTIFY_ACCESS_CODE not found");
            _spotifyToken = Environment.GetEnvironmentVariable("SPOTIFY_TOKEN") ?? throw new ArgumentNullException("SPOTIFY_TOKEN not found");
        }

        // Properties
        public string LoginURI => _loginURI;

        public string SpotifyUserID
        {
            get => _spotifyUserID;
            set => _spotifyUserID = value;
        }

        public string TokenExpireTime
        {
            get => _tokenExpireTime;
            set => _tokenExpireTime = value;
        }

        public SpotifyClient SpotifyClient
        {
            get => _spotifyClient;
            set => _spotifyClient = value;
        }

        public string SpotifyToken
        {
            get => _spotifyToken;
            set => _spotifyToken = value;
        }

        public string SpotifyAccessCode
        {
            get => _spotifyAccessCode;
            set => _spotifyAccessCode = value;
        }

        // Method to initiate Spotify login
        public Task InitiateSpotifyLoginAsync(HttpContext context)
        {
            var loginRequest = new LoginRequest(
                new Uri(_loginURI),
                _spotifyService.GetClientId(),
                LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] {
                    Scopes.PlaylistReadPrivate,
                    Scopes.PlaylistReadCollaborative,
                    Scopes.UserLibraryRead,
                    Scopes.UserReadRecentlyPlayed,
                    Scopes.UserTopRead
                }
            };

            var uri = loginRequest.ToUri();
            context.Response.Redirect(uri.ToString());
            return Task.CompletedTask;
        }

        // Method to get the Spotify client asynchronously
        public async Task<SpotifyClient> GetSpotifyClientAsync(string code)
        {
            SpotifyAccessCode = code;
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(
                    _spotifyService.GetClientId(),
                    _spotifyService.GetClientSecret(),
                    SpotifyAccessCode,
                    new Uri(_loginURI)
                )
            );

            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new AuthorizationCodeAuthenticator(
                    _spotifyService.GetClientId(),
                    _spotifyService.GetClientSecret(),
                    response
                ));

            SpotifyClient = new SpotifyClient(config);
            SpotifyToken = response.RefreshToken;
            TokenExpireTime = DateTime.Now.AddSeconds(response.ExpiresIn).ToString();

            return SpotifyClient;
        }

        // Method to refresh the Spotify token
        public async Task RefreshTokenAsync()
        {
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeRefreshRequest(
                    _spotifyService.GetClientId(),
                    _spotifyService.GetClientSecret(),
                    _spotifyToken
                )
            );

            SpotifyClient = new SpotifyClient(response.AccessToken);
            SpotifyToken = response.RefreshToken;
            TokenExpireTime = DateTime.Now.AddSeconds(response.ExpiresIn).ToString();
        }

        // Method to get recent tracks from Spotify
        public async Task<List<TrackDTO>> GetRecentTracksAsync(DateTimeOffset? startTime = null, DateTimeOffset? endTime = null, int trackCount = 10)
        {
            var spotify = SpotifyClient; // Use the SpotifyClient property instead of passing it in
            var recentlyPlayedRequest = new PlayerRecentlyPlayedRequest
            {
                Limit = trackCount
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

        // Method to check if the token is expiring in the next minute
        public bool IsTokenExpired()
        {
            return DateTime.Now > DateTime.Parse(TokenExpireTime) - TimeSpan.FromSeconds(60);
        }
    }
}
