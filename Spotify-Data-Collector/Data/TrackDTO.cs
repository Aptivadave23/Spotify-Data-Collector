using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Spotify_Data_Collector.Data;
using SpotifyDataCollector;

namespace SpotifyDataCollector{

    public class TrackDTO{
        public string Name { get; set; }
        public string SpotifyId { get; set; }
        public string Duration { get; set; }
        public string Popularity { get; set; }
        public string SpotifyUrl { get; set; }
        public string AlbumId { get; set; }
        public string ReleaseDate { get; set; }
        public string Disc_Number { get; set; }
        public string Track_Number { get; set; }

        public string ArtistId { get; set; }
        public string ArtistName { get; set; }
        


        public TrackDTO(string name, string spotifyId, string duration, string popularity, string spotifyUrl, string albumId, string releaseDate, string disc_Number, string track_Number, string artistId, string artistName)
        {
            Name = name;
            SpotifyId = spotifyId;
            Duration = duration;
            Popularity = popularity;
            SpotifyUrl = spotifyUrl;
            AlbumId = albumId;
            ReleaseDate = releaseDate;
            Disc_Number = disc_Number;
            Track_Number = track_Number;
            ArtistId = artistId;
            ArtistName = artistName;
        }
        
    }

    public class UserTrackDTO : TrackDTO {
        [JsonPropertyOrder(1000)]
        public string PlayedDateTime { get; set; }
    
        public UserTrackDTO(string name, string spotifyId, string duration, string popularity, string spotifyUrl, string albumId, string releaseDate, string disc_Number, string track_Number, string artistId, string artistName, string playedDateTime)
            : base(name, spotifyId, duration, popularity, spotifyUrl, albumId, releaseDate, disc_Number, track_Number, artistId, artistName)
        {
            PlayedDateTime = playedDateTime;
        }
    }
}