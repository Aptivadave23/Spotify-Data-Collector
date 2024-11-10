using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotifyAPI.Web;
using SpotifyDataCollector;


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

        // Method to get recent tracks from Spotify with pagination handling
        public async Task<List<UserTrackDTO>> GetTracksInTimeRangeAsync(DateTime startTime, DateTime endTime, int trackCount = 50)
        {
            var tracks = new List<UserTrackDTO>(); // Final list to return
            DateTime lastFetchedDate = startTime;

            Console.WriteLine($"Initial Start Time: {startTime}, End Time: {endTime}");

            // Set the initial after timestamp to null for the first request
            string afterTimestamp = null;

            while (lastFetchedDate <= endTime)
            {
                // Get recent tracks starting from the last fetched date or a specific cursor
                var recentTracksTemp = await GetRecentTracksAsync(lastFetchedDate, null, trackCount, afterTimestamp);

                if (!recentTracksTemp.Any())
                {
                    Console.WriteLine("No tracks found for the given time range.");
                    break;
                }

                Console.WriteLine($"Fetched {recentTracksTemp.Count()} tracks.");

                tracks.AddRange(recentTracksTemp);

                // If we fetched less than the requested track count, we've hit the end of available data
                if (recentTracksTemp.Count() < trackCount)
                {
                    break;
                }

                // Update lastFetchedDate to the time of the last fetched track
                lastFetchedDate = DateTime.Parse(recentTracksTemp.Last().PlayedDateTime).ToUniversalTime();
                Console.WriteLine($"Updated Last Date: {lastFetchedDate}");

                // Update the cursor for the next page of results
                afterTimestamp = recentTracksTemp.Last().PlayedDateTime;
            }

            return tracks;
        }

        // Method to get recent tracks from Spotify with startTime, endTime, and pagination
        public async Task<List<UserTrackDTO>> GetRecentTracksAsync(DateTime? startTime = null, DateTime? endTime = null, int trackCount = 10, string afterTimestamp = null)
        {
            var spotify = SpotifyClient; // Use the SpotifyClient property instead of passing it in
            var recentlyPlayedRequest = new PlayerRecentlyPlayedRequest
            {
                Limit = trackCount,
            };

            // Convert afterTimestamp (string) to long? (milliseconds since Unix epoch)
            if (!string.IsNullOrEmpty(afterTimestamp))
            {
                long afterTimestampMillis = new TimeZones().ConvertToUnixMilliseconds(DateTime.Parse(afterTimestamp));
                recentlyPlayedRequest.After = afterTimestampMillis;
            }

            // Convert startTime and endTime to milliseconds since Unix epoch if they are not null
            if (startTime.HasValue)
            {
                Console.WriteLine($"Start Time (Before Conversion): {startTime.Value}");
                recentlyPlayedRequest.After = new TimeZones().ConvertToUnixMilliseconds(startTime.Value);
                Console.WriteLine($"Start Time (After Conversion): {recentlyPlayedRequest.After}");
            }

            if (endTime.HasValue)
            {
                Console.WriteLine($"End Time (Before Conversion): {endTime.Value}");
                recentlyPlayedRequest.Before = new TimeZones().ConvertToUnixMilliseconds(endTime.Value);
                Console.WriteLine($"End Time (After Conversion): {recentlyPlayedRequest.Before}");
            }

            // Fetch recent tracks from Spotify
            var recentlyPlayed = await spotify.Player.GetRecentlyPlayed(recentlyPlayedRequest);
            Console.WriteLine($"Fetched {recentlyPlayed.Items.Count} tracks.");

            // Apply filtering directly before creating UserTrackDTO objects
            var tracks = recentlyPlayed.Items
                .Select(item =>
                {
                    // Ensure the PlayedAt time is in UTC
                    DateTime playedAtUtc = item.PlayedAt.ToUniversalTime(); // Ensure UTC first
                    Console.WriteLine($"Track PlayedAt (UTC): {playedAtUtc}");

                    // Print the startTime and endTime in milliseconds for debugging
                    long startTimeMillis = startTime.HasValue ? new TimeZones().ConvertToUnixMilliseconds(startTime.Value) : 0;
                    long endTimeMillis = endTime.HasValue ? new TimeZones().ConvertToUnixMilliseconds(endTime.Value) : 0;
                    Console.WriteLine($"Comparing with Start Time (Millis): {startTimeMillis}, End Time (Millis): {endTimeMillis}");

                    // Filter tracks by startTime and endTime
                    bool isTrackInRange = true;
                    if ((startTime.HasValue && playedAtUtc < startTime.Value.ToUniversalTime()) ||
                        (endTime.HasValue && playedAtUtc > endTime.Value.ToUniversalTime()))
                    {
                        isTrackInRange = false;
                        Console.WriteLine($"Track {item.Track.Name} skipped - PlayedAt {playedAtUtc} is outside the time range.");
                    }

                    // If the track is within the time range, return it
                    if (isTrackInRange)
                    {
                        return new UserTrackDTO(
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
                            item.Track.Artists[0].Name,
                            playedAtUtc.ToString() // Store the UTC time (or adjust to local time later)
                        );
                    }

                    return null;  // Skip track
                })
                .Where(track => track != null)  // Exclude null entries from skipped tracks
                .ToList();

            Console.WriteLine($"Filtered {tracks.Count} tracks within the specified time range.");
            return tracks;
        }




        // Method to check if the token is expiring in the next minute
        public bool IsTokenExpired()
        {
            return DateTime.Now > DateTime.Parse(TokenExpireTime) - TimeSpan.FromSeconds(60);
        }
    }
}
