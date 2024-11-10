using Carter;
using SpotifyUser; // Ensure this is the correct namespace for IUser
using Microsoft.AspNetCore.Mvc;
using SpotifyDataCollector;
using ResponseMessages;
using SpotifyAPI.Web;


public class UserEndPoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // Map the endpoint to retrieve recent tracks for the user
        app.MapGet("/user/RecentTracks/{trackCount:int?}", async (HttpContext context, [FromRoute] int? trackCount, [FromServices] IUser user) =>
        {
            // Check if the SpotifyClient is initialized and if the token is expired, refresh it
            if ((user.SpotifyClient == null) || (user.IsTokenExpired()))
            {
                await user.RefreshTokenAsync();
            }

            // Fetch recent tracks with a default of 10 tracks if trackCount is not specified
            var recentTracks = await user.GetRecentTracksAsync(null, DateTime.Now, trackCount: trackCount ?? 10);

            // Return the result in an Ok response
            return Results.Ok(recentTracks);
        })
        .Produces<List<UserTrackDTO>>(200, "application/json")
        .Produces<ErrorResponse>(400, "application/json")
        .Produces<ErrorResponse>(404, "application/json")
        .WithDisplayName("Get Recent Tracks")
        .WithSummary("Get the most recent tracks played by the user.")
        .WithDescription("Get the most recent tracks played by the user.")
        .WithTags("User Data");


        // Map the endpoint to retrieve tracks the user listened to in a specific time range
        app.MapPost("/user/TracksInTimeRange/", async (HttpContext context, [FromBody]TrackRequest tracksRequest, [FromServices] IUser user) =>
        {
            // Check if the SpotifyClient is initialized and if the token is expired, refresh it
            if ((user.SpotifyClient == null) || (user.IsTokenExpired()))
            {
                await user.RefreshTokenAsync();
            }

            // Fetch tracks listened to in the specified time range with a default of 10 tracks if trackCount is not specified
            var tracks = await user.GetTracksInTimeRangeAsync(tracksRequest.StartTime, tracksRequest.EndTime, 50);


            // Return the result in an Ok response
            return Results.Ok(tracks);
        })
        .WithDescription("Get tracks listened to in a specific time range.")
        .WithDisplayName("Get Tracks In Time Range")
        .WithSummary("Get tracks listened to in a specific time range.")
        .WithTags("User Data");
    }
}
