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
app.MapGet("/", () => {
    Console.WriteLine("Route Hit");
    var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
    return clientId + ' ' + clientSecret ;}
);

app.MapGet("/Spotify", async () => {
    var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

    var config = SpotifyClientConfig.CreateDefault();var request = new ClientCredentialsRequest(clientId, clientSecret);

            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));
            
    var me = await spotify.UserProfile.Current();
    Console.WriteLine($"Hello {me.DisplayName}!");
    }
);

app.Run();

