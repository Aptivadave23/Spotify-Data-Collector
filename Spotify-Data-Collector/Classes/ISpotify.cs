using Spotify_Data_Collector;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyDataCollector
{
    public interface ISpotifyService
    {
        /// <summary>
        /// Initializes the Spotify client with the client ID and client secret
        /// </summary>
        /// <returns>
        /// Returns the Spotify client
        /// </returns>
        Task InitializeClientAsync();

        /// <summary>
        /// Get the client ID.
        /// </summary>
        /// <returns></returns>
        string GetClientId();

        /// <summary>
        /// Get the client secret.
        /// </summary>
        /// <returns></returns>
        string GetClientSecret();

        /// <summary>
        /// Get artist details
        /// </summary>
        /// <param name="artistId">Spotify ID for the artist (Note:  This ID differs based on Country Code)</param>
        /// <returns>Artist DTO</returns>
        Task<ArtistDto> GetArtist(string artistId);

        /// <summary>
        /// Get album details
        /// </summary>
        /// <param name="albumId">Spotify ID for the album (Note:  This ID differs based on Country Code)</param>
        /// <returns>Album DTO</returns>
        Task<AlbumDto> GetAlbum(string albumId);

        /// <summary>
        /// Get playlist details (Not supported yet)
        /// </summary>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        Task<FullPlaylist> GetPlaylist(string playlistId);

        /// <summary>
        /// Get all albums for an artist
        /// </summary>
        /// <param name="artistId">Spotify ID for the artist (Note:  This is based on Country Code)</param>
        /// <returns>List of Album DTOs</returns>
        Task<List<AlbumDto>> GetArtistAlbums(string artistId);

        /// <summary>
        /// Search for an item, defaults to US market
        /// </summary>
        /// <param name="query">Item(s) searching for</param>
        /// <param name="searchType">Spotify Object Type (Album, Artist, Track, Playlist).  Non-user based objects</param>
        /// <param name="market">Two letter country code for the Spotify market to search in.</param>
        /// <returns>Search Response object</returns>
        Task<SearchResponse> Search(string query, SearchRequest.Types searchType, string market = "US");

        /// <summary>
        /// Get track details
        /// </summary>
        /// <param name="trackId">Spotify ID for the album (Note:  This ID differs based on Country Code)</param>
        /// <returns>Track DTO</returns>
        Task<TrackDTO> GetTrack(string trackId);

        /// <summary>
        /// Search for albums
        /// </summary>
        /// <param name="search">Search Term</param>
        /// <returns>List of Album DTOs</returns>
        Task<List<AlbumDto>> SearchAlbums(string search);

        /// <summary>
            /// Search for artists
            /// </summary>
            /// <param name="search">Search Term</param>
            /// <returns>LIst of Artist DTOs</returns>
        Task<List<ArtistDto>> SearchArtists(string search);

        /// <summary>
        /// Search for tracks
        /// </summary>
        /// <param name="search">Search Term</param>
        /// <returns>List of Track DTOs</returns>
        Task<List<TrackDTO>> SearchTracks(string search);
    }
}