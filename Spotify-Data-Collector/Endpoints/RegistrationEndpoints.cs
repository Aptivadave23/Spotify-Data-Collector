using Carter;
using Microsoft.AspNetCore.Mvc;
using SpotifyUser;

public class RegistrationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/login", async (HttpContext context, [FromServices]IUser user) =>
        {
            return user.InitiateSpotifyLoginAsync(context);
        })
        .Produces<OkResult>(200)
        .WithDisplayName("Login")
        .WithDescription("Initiate the Spotify login process.")
        .WithTags("Registration")
        .WithSummary("Initiate the Spotify login process.")
        .WithName("Login");

        app.MapGet("/redirect", async (HttpContext context, [FromServices] IUser user) =>
        {
            var code = context.Request.Query["code"].ToString();
            if (string.IsNullOrEmpty(code))
            {
                // Handle the missing or empty 'code' query parameter.
                // For example, return an error response to the client.
                return Results.BadRequest("The 'code' query parameter is required and cannot be empty.");
            }
            user.SpotifyAccessCode = code;

            // Retrieve the Spotify client using the provided code
            await user.GetSpotifyClientAsync(user.SpotifyAccessCode);

            // Retrieve the user profile
            var profile = await user.SpotifyClient.UserProfile.Current();
            user.SpotifyUserID = profile.Id;
            return Results.Ok(user.SpotifyUserID + ", " + user.SpotifyToken);
        })
        .WithDisplayName("Redirect")
        .WithDescription("Handle the redirect from Spotify after login.")
        .WithTags("Registration")
        .WithSummary("Handle the redirect from Spotify after login.")
        .WithName("Redirect");
    
    }
    
}