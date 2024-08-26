using System;
using SpotifyAPI.Web;
using System.Linq;
using Spotify_Data_Collector;

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

        public async Task<TrackDTO> GetTrack(string trackId)
        {
            var track = await spotifyClient.Tracks.Get(trackId);
            return new TrackDTO(track.Name, track.Id, track.DurationMs.ToString(), track.Popularity.ToString(), track.ExternalUrls["spotify"], track.Album.Id, track.Album.ReleaseDate, track.DiscNumber.ToString(), track.TrackNumber.ToString());
        }

        

        public async Task<AlbumDto> GetAlbum(string albumId)
        {
            var album = await spotifyClient.Albums.Get(albumId, new AlbumRequest{Market = "US"});
            return new AlbumDto(album.Name, album.Id, album.ReleaseDate, album.Images[0].Url,
                album.AlbumType.ToString(), album.TotalTracks.ToString(), "0", album.ExternalUrls.ToString(), album.Artists[0].Id, album.Artists[0].Name);
        }

        public async Task<FullPlaylist> GetPlaylist(string playlistId)
        {
            return await spotifyClient.Playlists.Get(playlistId);
        }

        public async Task<List<AlbumDto>> GetArtistAlbums(string artistId)
        {
            var pagingResult = await spotifyClient.Artists.GetAlbums(artistId, new ArtistsAlbumsRequest{Market = "US"});
            var albumDTOs = pagingResult.Items.Select(album => new AlbumDto(
                album.Name, album.Id, album.ReleaseDate, album.Images[0].Url,
                album.AlbumType, album.TotalTracks.ToString(), "0", album.ExternalUrls.ToString(), artistId, "Artist")).ToList();
           
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

        public async Task<List<AlbumDto>> SearchAlbums(string search)
        {
            var searchResults = await Search(search, SearchRequest.Types.Album);

            var Albums = searchResults.Albums.Items.ToList();

            List<AlbumDto> albums = new List<AlbumDto>();

            if (Albums.Count() > 0)   
            {
            
                foreach (var a in Albums){
                    var albumDetails = await GetAlbum(a.Id);
                    var albumDto = new AlbumDto(
                        albumDetails.Name, 
                        albumDetails.Id, 
                        albumDetails.ReleaseDate, 
                        albumDetails.ImageUrl,
                        albumDetails.AlbumType,
                        albumDetails.TotalTracks,
                        albumDetails.Popularity,
                        albumDetails.SpotifyUrl,
                        albumDetails.ArtistId,
                        albumDetails.ArtistName
                        );
                    albums.Add(albumDto);
                }
                }
                return albums;
                
            }
    }
}