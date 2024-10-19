using Carter;
using SpotifyDataCollector;
using Microsoft.AspNetCore.Mvc;

public class SpotifyObjectsEndPoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/Spotify", async ([FromServices] ISpotifyService spotify) =>
        {
            return spotify.ToString();
        });

        app.MapGet("/Spotify/Search/Artist/{search}", async (HttpContext context, [FromRoute] string search, [FromServices] ISpotifyService spotifyService) =>
        {
            var artists = await spotifyService.SearchArtists(search);
            return Results.Ok(artists.ToList());
        })
        .WithDescription("Search for artists by name")
        .WithDisplayName("Search Artists")
        .WithSummary("Search for artists by name")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Search/Album/{search}", async (HttpContext context, [FromRoute] string search, [FromServices] ISpotifyService spotifyService) =>
        {
            var albums = await spotifyService.SearchAlbums(search);
            return Results.Ok(albums.ToList());
        })
        .WithDescription("Search for albums by name")
        .WithDisplayName("Search Albums")
        .WithSummary("Search for albums by name")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Search/Track/{search}", async (HttpContext context, [FromRoute] string search, [FromServices] ISpotifyService spotifyService) =>
        {
            var tracks = await spotifyService.SearchTracks(search);
            return Results.Ok(tracks.ToList());
        })
        .WithDescription("Search for tracks by name")
        .WithDisplayName("Search Tracks")
        .WithSummary("Search for tracks by name")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Album/{id}", async (HttpContext context, [FromRoute] string id, [FromServices] ISpotifyService spotifyService) =>
        {
            var album = await spotifyService.GetAlbum(id);
            return Results.Ok(album);
        })
        .WithDescription("Get an album by ID")
        .WithDisplayName("Get Album")
        .WithSummary("Get an album by ID")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Artist/{id}", async (HttpContext context, [FromRoute] string id, [FromServices] ISpotifyService spotifyService) =>
        {
            var artist = await spotifyService.GetArtist(id);
            return Results.Ok(artist);
        })
        .WithDescription("Get an artist by ID")
        .WithDisplayName("Get Artist")
        .WithSummary("Get an artist by ID")
        .WithTags("Spotify");

        app.MapGet("/Spotify/Track/{id}", async (HttpContext context, [FromRoute] string id, [FromServices] ISpotifyService spotifyService) =>
        {
            var track = await spotifyService.GetTrack(id);
            return Results.Ok(track);
        })
        .WithDescription("Get a track by ID")
        .WithDisplayName("Get Track")
        .WithSummary("Get a track by ID")
        .WithTags("Spotify");
    }
}
