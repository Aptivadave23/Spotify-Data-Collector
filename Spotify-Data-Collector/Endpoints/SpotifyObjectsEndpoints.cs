using Carter;
using SpotifyDataCollector;  // Ensure this is included to reference your DTOs
using Microsoft.AspNetCore.Mvc;
using ResponseMessages;

public class SpotifyObjectsEndPoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Spotify/Search/Artist/{search}", async (HttpContext context, [FromRoute] string search, [FromServices] ISpotifyService spotifyService) =>
        {
            var artists = await spotifyService.SearchArtists(search);
            if (artists == null)
            {
                return Results.NotFound(new ErrorResponse { StatusCode = 404, Message = "Artists not found", Details = "No artists found for the given search query" });
            }
            else
                return Results.Ok(artists.ToList());
        })
        .Produces<List<SpotifyDataCollector.ArtistDto>>(200, "application/json")  // Using your DTO for Artist
        .ProducesProblem(400)
        .Produces<ResponseMessages.ErrorResponse>(404, "application/json")
        .WithDescription("Search for artists by name")
        .WithDisplayName("Search Artists")
        .WithSummary("Search for artists by name")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Search/Album/{search}", async (HttpContext context, [FromRoute] string search, [FromServices] ISpotifyService spotifyService) =>
        {
            var albums = await spotifyService.SearchAlbums(search);
            if (albums == null)
            {
                return Results.NotFound(new ErrorResponse { StatusCode = 404, Message = "Albums not found", Details = "No albums found for the given search query" });
            }
            else
                return Results.Ok(albums.ToList());
        })
        .Produces<List<SpotifyDataCollector.AlbumDto>>(200, "application/json")  // Using your DTO for Album
        .ProducesProblem(400)
        .Produces<ResponseMessages.ErrorResponse>(404, "application/json")
        .WithDescription("Search for albums by name")
        .WithDisplayName("Search Albums")
        .WithSummary("Search for albums by name")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Search/Track/{search}", async (HttpContext context, [FromRoute] string search, [FromServices] ISpotifyService spotifyService) =>
        {
            var tracks = await spotifyService.SearchTracks(search);
            if (tracks == null)
            {
                return Results.NotFound(new ErrorResponse { StatusCode = 404, Message = "Tracks not found", Details = "No tracks found for the given search query" });
            }
            else
                return Results.Ok(tracks.ToList());
        })
        .Produces<List<SpotifyDataCollector.TrackDTO>>(200, "application/json")  // Using your DTO for Track
        .ProducesProblem(400)
        .Produces<ResponseMessages.ErrorResponse>(404, "application/json")
        .WithDescription("Search for tracks by name")
        .WithDisplayName("Search Tracks")
        .WithSummary("Search for tracks by name")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Album/{id}", async (HttpContext context, [FromRoute] string id, [FromServices] ISpotifyService spotifyService) =>
        {
            var album = await spotifyService.GetAlbum(id);
            if (album == null)
            {
                return Results.NotFound(new ErrorResponse { StatusCode = 404, Message = "Album not found", Details = "No album found for the given ID" });
            }
            else
                return Results.Ok(album);
        })
        .Produces<SpotifyDataCollector.AlbumDto>(200, "application/json")  // Using your DTO for Album
        .ProducesProblem(400)
        .Produces<ResponseMessages.ErrorResponse>(404, "application/json")
        .WithDescription("Get an album by ID")
        .WithDisplayName("Get Album")
        .WithSummary("Get an album by ID")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Artist/{id}", async (HttpContext context, [FromRoute] string id, [FromServices] ISpotifyService spotifyService) =>
        {
            var artist = await spotifyService.GetArtist(id);
            if (artist == null)
            {
                return Results.NotFound(new ErrorResponse { StatusCode = 404, Message = "Artist not found", Details = "No artist found for the given ID" });
            }
            else
                return Results.Ok(artist);
        })
        .Produces<SpotifyDataCollector.ArtistDto>(200, "application/json")  // Using your DTO for Artist
        .ProducesProblem(400)
        .Produces<ResponseMessages.ErrorResponse>(404, "application/json")
        .WithDescription("Get an artist by ID")
        .WithDisplayName("Get Artist")
        .WithSummary("Get an artist by ID")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Track/{id}", async (HttpContext context, [FromRoute] string id, [FromServices] ISpotifyService spotifyService) =>
        {
            var track = await spotifyService.GetTrack(id);
            if (track == null)
            {
                return Results.NotFound(new ErrorResponse { StatusCode = 404, Message = "Track not found", Details = "No track found for the given ID" });
            }
            else
                return Results.Ok(track);
        })
        .Produces<SpotifyDataCollector.TrackDTO>(200, "application/json")  // Using your DTO for Track
        .ProducesProblem(400)
        .Produces<ResponseMessages.ErrorResponse>(404, "application/json")
        .WithDescription("Get a track by ID")
        .WithDisplayName("Get Track")
        .WithSummary("Get a track by ID")
        .WithTags("Spotify");
    }
}
