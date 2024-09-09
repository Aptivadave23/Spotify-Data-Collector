using SpotifyAPI.Web;
using SpotifyUser;
using SpotifyDataCollector;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;



DotEnv.Load();
var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Spotify>();
builder.Services.AddScoped<ISpotifyService, Spotify>();
builder.Services.AddScoped<IUser, User>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(Options =>
{
    Options.Cookie.Name = "SpotifySession";
    Options.IdleTimeout = TimeSpan.FromMinutes(30);
    Options.Cookie.IsEssential = true;
    Options.Cookie.HttpOnly = true;
});
var app = builder.Build();
app.UseSession();
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
    
    var code = context.Request.Query["code"].ToString();
    await user.GetSpotifyClientAsync(code);
    var profile = await user.SpotifyClient.UserProfile.Current();
    user.SpotifyUserID = profile.Id;
    if(context.Session.GetString("GoBackRoute") != null)
    {
        return Results.Redirect(context.Session.GetString("GoBackRoute"));
    }
    else
        return Results.Ok(user.TokenExpireTime);
}
);
app.MapGet("/user/RecentTracks/{trackCount?}", async (HttpContext context) =>
{
    // Check to see if the user has a Spotify client
    if (user.SpotifyClient == null)
    {
        // Combine the request path and query string to capture the full route
        var fullRoute = $"{context.Request.Path}{context.Request.QueryString}";

        // Set the GoBackRoute session variable to the full route
        context.Session.SetString("GoBackRoute", fullRoute);   
        return Results.Redirect("/login");
    }
    else if (user.IsTokenExpired())
    {
        await user.RefreshTokenAsync();
    }

    // Attempt to parse trackCount, default to 10 if parsing fails or if value is not positive
    var trackCountStr = context.Request.RouteValues["trackCount"]?.ToString() ?? "10";
    if (!Int16.TryParse(trackCountStr, out short trackCount) || trackCount <= 0)
    {
        trackCount = 10;
    }
    else if (trackCount > 50)
        return Results.BadRequest("Track count must be less than or equal to 50");


    var tracks = await user.GetRecentTracksAsync(user.SpotifyClient, null, DateTime.Now, trackCount);
    if ((tracks == null) || (tracks.Count == 0))
    {
        return Results.BadRequest("No tracks found");
    }
    else
        return Results.Ok(tracks.ToList());
});

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

