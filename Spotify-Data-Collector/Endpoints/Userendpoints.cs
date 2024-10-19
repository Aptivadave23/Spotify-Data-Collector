using Carter;
using SpotifyUser; // Ensure this is the correct namespace for IUser


public class UserEndPoints : ICarterModule
{
    // Remove the constructor-level dependency injection
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/user/RecentTracks/{trackCount:int?}", async (HttpContext context, int? trackCount, IUser user) =>
        {
            // Access the IUser instance from the request's service provider (dependency injection)
            //var user = context.RequestServices.GetRequiredService<IUser>();
           
            // Check if the SpotifyClient is initialized
             // Check to see if the user has a Spotify client
            if ((user.SpotifyClient == null) || (user.IsTokenExpired()))
            {
                await user.RefreshTokenAsync();
            }

            


            // Here, call the method that uses the SpotifyClient, for example:
            // Fetch recent tracks, with a default of 10 tracks if not specified
            var recentTracks = await user.GetRecentTracksAsync(null, DateTime.Now,trackCount: trackCount ?? 10);

            return Results.Ok(recentTracks);
        })
        .WithDisplayName("Get Recent Tracks")
        .WithSummary("Get the most recent tracks played by the user.")
        .WithDescription("Get the most recent tracks played by the user.")
        .WithTags("User Data");
    }
}
