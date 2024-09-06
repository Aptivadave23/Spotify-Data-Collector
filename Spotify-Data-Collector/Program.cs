using SpotifyAPI.Web;
using SpotifyUser;
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
builder.Services.AddScoped<IUser, User>();

var app = builder.Build();

// Initialize the Spotify client
var serviceProvider = app.Services.CreateScope().ServiceProvider;
var spotifyService = serviceProvider.GetRequiredService<ISpotifyService>();
spotifyService.InitializeClientAsync().Wait();
// Initialize the Spotify user
var user = serviceProvider.GetRequiredService<IUser>();
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


//Spotify User Routes
app.MapGet("/login", (HttpContext context) =>
{
    return user.InitiateSpotifyLoginAsync(context);
}
);
app.MapGet("/redirect", async (HttpContext context) =>
{
    //todo: check to see if we already have the access code and token.  If we don't, get it.  Otherwise, refresh the token if needed.
    var code = context.Request.Query["code"].ToString();
    await user.GetSpotifyClientAsync(code);
    var profile = await user.SpotifyClient.UserProfile.Current();
    user.SpotifyUserID = profile.Id;
    return Results.Ok(user.TokenExpireTime);
}
);
app.MapGet("/user/RecentTracks", async (HttpContext context) =>
{
    //check to see if the user has a spotify client
    if(user.SpotifyClient == null)
    {
       await user.InitiateSpotifyLoginAsync(context);
       user.SpotifyClient = await user.GetSpotifyClientAsync(user.SpotifyAccessCode);
    }
    else if(user.IsTokenExpired())
    {
        await user.RefreshTokenAsync();
    }
    var tracks = await user.GetRecentTracksAsync(user.SpotifyClient, null, DateTime.Now);
    return Results.Ok(tracks.ToList());
}
);

//Spotify Data Collection Routes
app.MapGet("/Spotify", async (ISpotifyService spotify) =>
{
    //await spotify.InitializeClientAsync();

    return spotify.ToString();
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

