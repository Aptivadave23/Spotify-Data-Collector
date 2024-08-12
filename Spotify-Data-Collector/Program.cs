using SpotifyAPI.Web;
using SpotifyDataCollector;
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
    var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
    var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
    return clientId.ToString();
}
);

app.MapGet("/Spotify", async () =>
{
    var clientId = "{ClientId}";
    var clientSecret = "{clientSecret}";
Console.WriteLine(clientId + " " + clientSecret);
    var config = SpotifyClientConfig
        .CreateDefault()
        .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));
    var spotify = new SpotifyClient(config);
    var user = await spotify.UserProfile.Current();
    var response = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, "Never Gonna Give You Up"));
    return user.DisplayName;
}
);

app.Run();

