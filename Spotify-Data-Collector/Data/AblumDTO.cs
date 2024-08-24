using System;
namespace SpotifyDataCollector
{
    public class AlbumDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ReleaseDate { get; set; }
        public string ImageUrl { get; set; }
        public AlbumDto(string name, string id, string releaseDate, string imageUrl)
        {
            Name = name;
            Id = id;
            ReleaseDate = releaseDate;
            ImageUrl = imageUrl;
        }
    }
}