using Spotify_Data_Collector;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotifyDataCollector
{
    public interface ISpotifyService
    {
        Task InitializeClientAsync();
        Task<FullArtist> GetArtist(string artistId);
        Task<AlbumDto> GetAlbum(string albumId);
        Task<FullPlaylist> GetPlaylist(string playlistId);
        Task<List<AlbumDto>> GetArtistAlbums(string artistId);
        Task<SearchResponse> Search(string query, SearchRequest.Types searchType, string market = "US");
        Task<TrackDTO> GetTrack(string trackId);
        Task<List<AlbumDto>> SearchAlbums(string search);
    }
}