using System;
using SpotifyAPI.Web;
using System.Linq;

namespace SpotifyDataCollector
{
    public class Spotify : ISpotifyService
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

        public async Task<AlbumDto> GetAlbum(string albumId)
        {
            var album = await spotifyClient.Albums.Get(albumId, new AlbumRequest{Market = "US"});
            return new AlbumDto(album.Name, album.Id, album.ReleaseDate, album.Images[0].Url);
        }

        public async Task<FullPlaylist> GetPlaylist(string playlistId)
        {
            return await spotifyClient.Playlists.Get(playlistId);
        }

        public async Task<List<AlbumDto>> GetArtistAlbums(string artistId)
        {
            var pagingResult = await spotifyClient.Artists.GetAlbums(artistId, new ArtistsAlbumsRequest{Market = "US"});
            //var albumDTOs = pagingResult.Items.Select(album => new AlbumDto(album.Name, album.Id, album.ReleaseDate, album.Images[0].Url)).ToList();
            var albumDTOs = pagingResult.Items.Select(album => new AlbumDto(album.Name, album.Id, album.ReleaseDate, album.Images[0].Url)).ToList();

            return albumDTOs;
        }

        public async Task<SearchResponse> Search(string query, SearchRequest.Types searchType, string market = "US")
        {
            var searchRequest = new SearchRequest(searchType, query)
            {
                Market = market
            };

            return await spotifyClient.Search.Item(searchRequest);  
        }
        
    }
}