using SpotifyAPI.Web;
using SpotifyDataCollector;
using dotenv.net;


DotEnv.Load();
var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

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

app.MapGet("/Spotify", async () =>
{
   
Console.WriteLine(clientId + " " + clientSecret);
    var config = SpotifyClientConfig
        .CreateDefault()
        .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));
    var request = new ClientCredentialsRequest(clientId, clientSecret);
    var response = await new OAuthClient(config).RequestToken(request);

    var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

    var artist = await spotify.Artists.Get("0OdUWJ0sBjDrqHygGUXeCF");

    return artist.Name;
}
);

app.MapGet("/Spotify/Search/Artist/{search}", async (string search) =>
{
    var config = SpotifyClientConfig
        .CreateDefault()
        .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));
    var request = new ClientCredentialsRequest(clientId, clientSecret);
    var response = await new OAuthClient(config).RequestToken(request);

    var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

    var searchResults = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Artist, search));

    return searchResults.Artists.Items.ToList();
}
);

app.Run();

