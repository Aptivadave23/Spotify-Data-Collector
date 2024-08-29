using System;
using Spotify_Data_Collector;
namespace SpotifyDataCollector
{
    public class AlbumDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ReleaseDate { get; set; }
        public string ImageUrl { get; set; }
        public string AlbumType { get; set; }
        public string TotalTracks { get; set; }
        public string Popularity { get; set; }
        public string SpotifyUrl { get; set; }
        public string ArtistId { get; set; }    
        public string ArtistName { get; set; }

        public AlbumDto(string name, string id, string releaseDate, string imageUrl, string albumType, string totalTracks, string popularity, string spotifyUrl, string artistId, string artistName)
        
        {
            Name = name;
            Id = id;
            ReleaseDate = releaseDate;
            ImageUrl = imageUrl;
            AlbumType = albumType;
            TotalTracks = totalTracks;
            Popularity = popularity;
            SpotifyUrl = spotifyUrl;
            ArtistId = artistId;
            ArtistName = artistName;
        
        }
    }
}