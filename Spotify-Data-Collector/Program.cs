using SpotifyAPI.Web;
using SpotifyDataCollector;
using dotenv.net;


DotEnv.Load();
var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Spotify>();
builder.Services.AddScoped<ISpotifyService, Spotify>();

var app = builder.Build();




/*static async Task Main(string[] args)
    {
        ISpotifyService spotifyService = new Spotify();
        await spotifyService.InitializeClientAsync();       
        
    }*/

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

app.MapGet("/Spotify/Search/Artist/{search}", async (Microsoft.AspNetCore.Http.HttpContext context, string search) =>
{    

    var searchResults = await spotifyService.Search(search, SearchRequest.Types.Artist);

    var Artists = searchResults.Artists.Items.ToList();

    if (Artists.Count() == 0)
    {
        return Results.NotFound();
    }
    else 
    {
        List<ArtistDto> artistDto = new List<ArtistDto>();
        foreach (var artist in Artists)
        {
        var artistDetails = await spotifyService.GetArtist(artist.Id);
        var artistAlbums = await spotifyService.GetArtistAlbums(artist.Id);
        var discography = artistAlbums.Select(album => new AlbumDto(album.Name, album.Id, album.ReleaseDate, album.ImageUrl)).ToList();
        artistDto.Add(new ArtistDto(
            artistDetails.Name, 
            artistDetails.Id, 
            artistDetails.Genres, 
            artistDetails.ExternalUrls["spotify"],
            discography
            ));
        }
        

        return Results.Ok(artistDto);
    }
}
);

app.MapGet("/Spotify/Search/Album/{search}", async (Microsoft.AspNetCore.Http.HttpContext context, string search) =>
{
    var searchResults = await spotifyService.Search(search, SearchRequest.Types.Album);

    var Albums = searchResults.Albums.Items.ToList();

    

    if (Albums.Count() == 0)
    {
        return Results.NotFound();
    }
    else 
    {
        List<AlbumDto> albums = new List<AlbumDto>();
        foreach (var a in Albums){
            var albumDetails = await spotifyService.GetAlbum(a.Id);
            var albumDto = new AlbumDto(
                albumDetails.Name, 
                albumDetails.Id, 
                albumDetails.ReleaseDate, 
                albumDetails.ImageUrl
                );
            albums.Add(albumDto);
        }

        return Results.Ok(albums.ToList());
    }
}
);

app.Run();

