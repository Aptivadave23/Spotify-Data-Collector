using System;
using SpotifyAPI.Web;

namespace SpotifyDataCollector
{
    public class Spotify
    {
        // Add your class members and methods here
        private string clientId { get; set; }
        private string clientSecret { get; set; }
        private SpotifyClient spotifyClient;

        public Spotify()
        {
            // Implement constructor logic here
            clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
           
        }
        public async Task InitializeClientAsync()
        {
            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));
            var request = new ClientCredentialsRequest(clientId, clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            spotifyClient = new SpotifyClient(config.WithToken(response.AccessToken));
        }
        public async Task<FullArtist> GetArtist(string artistId)
        {
            return await spotifyClient.Artists.Get(artistId);
        }

        public async Task<FullTrack> GetTrack(string trackId)
        {
            return await spotifyClient.Tracks.Get(trackId);
        }

        public async Task<FullAlbum> GetAlbum(string albumId)
        {
            return await spotifyClient.Albums.Get(albumId);
        }

        public async Task<FullPlaylist> GetPlaylist(string playlistId)
        {
            return await spotifyClient.Playlists.Get(playlistId);
        }

        public async Task<SearchResponse> Search(string query)
        {
            return await spotifyClient.Search.Item(new SearchRequest(SearchRequest.Types.All, query));
        }
        
    }
}