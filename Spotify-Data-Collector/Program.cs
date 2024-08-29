using SpotifyAPI.Web;
using SpotifyDataCollector;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;



DotEnv.Load();
var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Spotify>();
builder.Services.AddScoped<ISpotifyService, Spotify>();

var app = builder.Build();

// Initialize the Spotify client
var serviceProvider = app.Services.CreateScope().ServiceProvider;
var spotifyService = serviceProvider.GetRequiredService<ISpotifyService>();
spotifyService.InitializeClientAsync().Wait();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseStaticFiles();
}


//hello world route
app.MapGet("/", () =>
{
    Console.WriteLine("Route Hit");
    
    return clientId.ToString();
}
);

app.MapGet("/Spotify", async (ISpotifyService spotify) =>
{
    //await spotify.InitializeClientAsync();

    return spotify.ToString();
}
);

app.MapGet("/login", (HttpContext context) =>
{
   // Make sure "http://localhost:5543/callback" is in your application's redirect URIs!
    var loginRequest = new LoginRequest(
      new Uri("http://localhost:5272/redirect"), // The redirect URI where Spotify will send the authorization code
      spotifyService.GetClientId(), // Replace with your Spotify Client ID
      LoginRequest.ResponseType.Code
    )
    {
        Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
    };
    var uri = loginRequest.ToUri();
    context.Response.Redirect(uri.ToString()); // Redirect user to Spotify login
    return Task.CompletedTask;
}
);
app.MapGet("/redirect", async (HttpContext context) =>
{
    var code = context.Request.Query["code"].ToString();
    var response = await new OAuthClient().RequestToken(
      new AuthorizationCodeTokenRequest(
        spotifyService.GetClientId(), // Replace with your Spotify Client ID
        spotifyService.GetClientSecret(), // Replace with your Spotify Client Secret
        code,
        new Uri("http://localhost:5272/redirect")
      )
    );
    var spotify = new SpotifyClient(response.AccessToken);
    var profile = await spotify.UserProfile.Current();
    return Results.Ok(profile.DisplayName);
}
);

app.MapGet("/Spotify/Search/Artist/{search}", async (Microsoft.AspNetCore.Http.HttpContext context, string search) =>
{    
    var artists = await spotifyService.SearchArtists(search);
    return Results.Ok(artists.ToList());
    
});


app.MapGet("/Spotify/Search/Album/{search}", async (Microsoft.AspNetCore.Http.HttpContext context, string search) =>
{
        var albums = await spotifyService.SearchAlbums(search);
        return Results.Ok(albums.ToList());
    
});

app.MapGet("/Spotify/Search/Track/{search}", async (Microsoft.AspNetCore.Http.HttpContext context, string search) =>
{
    var tracks = await spotifyService.SearchTracks(search);
    return Results.Ok(tracks.ToList());
}
);

app.MapGet("/Spotify/Album/{id}", async (Microsoft.AspNetCore.Http.HttpContext context, string id) =>
{
    var album = await spotifyService.GetAlbum(id);
    return Results.Ok(album);
}
);



app.MapGet("/Spotify/Artist/{id}", async (Microsoft.AspNetCore.Http.HttpContext context, string id) =>
{
    var artist = await spotifyService.GetArtist(id);
    return Results.Ok(artist);
}
);

app.MapGet("/Spotify/Track/{id}", async (Microsoft.AspNetCore.Http.HttpContext context, string id) =>
{
    var track = await spotifyService.GetTrack(id);
    return Results.Ok(track);
}
);

app.Run();

