using System;
namespace SpotifyDataCollector
{
    public class ArtistDto
    {
        public string Name { get; set; }
        public string SpotifyId { get; set; }
        public List<string> Genres { get; set; }
        public string SpotifyUrl { get; set; }
        public List<AlbumDto> Discography { get; set; }
        public ArtistDto(string name, string spotifyId, List<string> genres, string spotifyUrl, List<AlbumDto> discography)
        {
            Name = name;
            SpotifyId = spotifyId;
            Genres = genres ?? new List<string>();
            SpotifyUrl = spotifyUrl;
            Discography = discography ?? new List<AlbumDto>();
        }
    }
}