using System;
using SpotifyAPI.Web;
using System.Linq;
using Spotify_Data_Collector;

namespace SpotifyDataCollector
{
    public class Spotify : ISpotifyService
    {
        /// <summary>
        /// Represents a class that interacts with the Spotify API.
        /// </summary>
        public Spotify()
        {
            clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new InvalidOperationException("Spotify Client ID or Secret is missing. Please check your environment variables.");
            }
        }
        // Add your class members and methods here

        private string clientId { get; set; }
        private string clientSecret { get; set; }
        private SpotifyClient spotifyClient;

        /// <summary>
        /// Get the client ID.
        /// </summary>
        /// <returns></returns>
        public string GetClientId()
        {
            return clientId;
        }
        /// <summary>
        /// Get the client secret.
        /// </summary>
        /// <returns></returns>
        public string GetClientSecret()
        {
            return clientSecret;
        }
         /// <summary>
        /// Initializes the Spotify client asynchronously.
        /// </summary>
        public async Task InitializeClientAsync()
        {
            if (spotifyClient == null)
            {
                var config = SpotifyClientConfig
                    .CreateDefault()
                    .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));

                var request = new ClientCredentialsRequest(clientId, clientSecret);
                var response = await new OAuthClient(config).RequestToken(request);

                spotifyClient = new SpotifyClient(config.WithToken(response.AccessToken));
            }
        }

        /// <summary>
        /// Ensures that the Spotify client has been initialized before usage.
        /// </summary>
        private async Task EnsureClientInitializedAsync()
        {
            if (spotifyClient == null)
            {
                await InitializeClientAsync();
            }
        }


        /// <summary>
        /// Get artist details.
        /// </summary>
        /// <param name="artistId">Spotify ID for the artist (Note: This ID differs based on Country Code).</param>
        /// <returns>Artist DTO.</returns>
        public async Task<ArtistDto> GetArtist(string artistId)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var artist = await spotifyClient.Artists.Get(artistId);
            return new ArtistDto(artist.Name, artist.Id, artist.Genres, artist.ExternalUrls["spotify"], artist.Popularity.ToString());
        }

        /// <summary>
        /// Get track details.
        /// </summary>
        /// <param name="trackId">Spotify ID for the album (Note: This ID differs based on Country Code).</param>
        /// <returns>Track DTO.</returns>
        public async Task<TrackDTO> GetTrack(string trackId)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var track = await spotifyClient.Tracks.Get(trackId);
            return new TrackDTO(track.Name, track.Id, track.DurationMs.ToString(), track.Popularity.ToString(), track.ExternalUrls["spotify"], track.Album.Id, track.Album.ReleaseDate, track.DiscNumber.ToString(), track.TrackNumber.ToString(), track.Artists[0].Id, track.Artists[0].Name);
        }

        /// <summary>
        /// Get album details.
        /// </summary>
        /// <param name="albumId">Spotify ID for the album (Note: This ID differs based on Country Code).</param>
        /// <returns>Album DTO.</returns>
        public async Task<AlbumDto> GetAlbum(string albumId)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var album = await spotifyClient.Albums.Get(albumId, new AlbumRequest { Market = "US" });
            return new AlbumDto(album.Name, album.Id, album.ReleaseDate, album.Images[0].Url,
                album.AlbumType.ToString(), album.TotalTracks.ToString(), "0", album.ExternalUrls["spotify"].ToString(), album.Artists[0].Id, album.Artists[0].Name);
        }

        /// <summary>
        /// Get playlist details (Not supported yet).
        /// </summary>
        /// <param name="playlistId">Spotify ID for the playlist.</param>
        /// <returns>FullPlaylist object.</returns>
        public async Task<FullPlaylist> GetPlaylist(string playlistId)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            return await spotifyClient.Playlists.Get(playlistId);
        }

        /// <summary>
        /// Get all albums for an artist.
        /// </summary>
        /// <param name="artistId">Spotify ID for the artist (Note: This is based on Country Code).</param>
        /// <returns>List of Album DTOs.</returns>
        public async Task<List<AlbumDto>> GetArtistAlbums(string artistId)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var pagingResult = await spotifyClient.Artists.GetAlbums(artistId, new ArtistsAlbumsRequest { Market = "US" });
            var albumDTOs = pagingResult.Items.Select(album => new AlbumDto(
                album.Name, album.Id, album.ReleaseDate, album.Images[0].Url,
                album.AlbumType, album.TotalTracks.ToString(), "0", album.ExternalUrls.ToString(), artistId, "Artist")).ToList();

            return albumDTOs;
        }

        /// <summary>
        /// Search for an item, defaults to US market.
        /// </summary>
        /// <param name="query">Item(s) searching for.</param>
        /// <param name="searchType">Spotify Object Type (Album, Artist, Track, Playlist). Non-user based objects.</param>
        /// <param name="market">Two letter country code for the Spotify market to search in.</param>
        /// <returns>Search Response object.</returns>
        public async Task<SearchResponse> Search(string query, SearchRequest.Types searchType, string market = "US")
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var searchRequest = new SearchRequest(searchType, query)
            {
                Market = market
            };

            return await spotifyClient.Search.Item(searchRequest);
        }

        /// <summary>
        /// Search for albums.
        /// </summary>
        /// <param name="search">Search Term.</param>
        /// <returns>List of Album DTOs.</returns>
        public async Task<List<AlbumDto>> SearchAlbums(string search)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var searchResults = await Search(search, SearchRequest.Types.Album);

            var Albums = searchResults.Albums.Items.ToList();

            List<AlbumDto> albums = new List<AlbumDto>();

            if (Albums.Count() > 0)
            {
                foreach (var a in Albums)
                {
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

        /// <summary>
        /// Search for artists.
        /// </summary>
        /// <param name="search">Search Term.</param>
        /// <returns>List of Artist DTOs.</returns>
        public async Task<List<ArtistDto>> SearchArtists(string search)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var searchResults = await Search(search, SearchRequest.Types.Artist);

            var Artists = searchResults.Artists.Items.ToList();

            List<ArtistDto> artists = new List<ArtistDto>();

            if (Artists.Count() > 0)
            {
                foreach (var a in Artists)
                {
                    var artistDetails = await GetArtist(a.Id);
                    var artistDto = new ArtistDto(
                        artistDetails.Name,
                        artistDetails.SpotifyId,
                        artistDetails.Genres,
                        artistDetails.SpotifyUrl,
                        artistDetails.Popularity.ToString()
                    );
                    artists.Add(artistDto);
                }
            }
            return artists;
        }

        /// <summary>
        /// Search for tracks.
        /// </summary>
        /// <param name="search">Search Term.</param>
        /// <returns>List of Track DTOs.</returns>
        public async Task<List<TrackDTO>> SearchTracks(string search)
        {
            await EnsureClientInitializedAsync(); // Ensure the client is initialized
            var searchResults = await Search(search, SearchRequest.Types.Track);

            var Tracks = searchResults.Tracks.Items.ToList();

            List<TrackDTO> tracks = new List<TrackDTO>();

            if (Tracks.Count() > 0)
            {
                foreach (var t in Tracks)
                {
                    var trackDetails = await GetTrack(t.Id);
                    var trackDto = new TrackDTO(
                        trackDetails.Name,
                        trackDetails.SpotifyId,
                        trackDetails.Duration,
                        trackDetails.Popularity,
                        trackDetails.SpotifyUrl,
                        trackDetails.AlbumId,
                        trackDetails.ReleaseDate,
                        trackDetails.Disc_Number,
                        trackDetails.Track_Number,
                        trackDetails.ArtistId,
                        trackDetails.ArtistName
                    );
                    tracks.Add(trackDto);
                }
            }
            return tracks;
        }
    }
}