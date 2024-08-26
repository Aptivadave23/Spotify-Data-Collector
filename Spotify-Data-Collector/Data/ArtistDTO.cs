using System;
using System.Text.Json.Serialization;
namespace SpotifyDataCollector
{
    public class ArtistDto
    {
        [JsonPropertyOrder(1)]
        public string Name { get; set; }
        [JsonPropertyOrder(2)]
        public string SpotifyId { get; set; }
        [JsonPropertyOrder(3)]
        public List<string> Genres { get; set; }
        [JsonPropertyOrder(4)]
        public string Popularity { get; set; }
        [JsonPropertyOrder(5)]
        public string SpotifyUrl { get; set; }
        
        
        public ArtistDto(string name, string spotifyId, List<string> genres, string spotifyUrl,  string popularity)
        {
            Name = name;
            SpotifyId = spotifyId;
            Genres = genres ?? new List<string>();
            SpotifyUrl = spotifyUrl;
            Popularity = popularity;       
        }
    }
}