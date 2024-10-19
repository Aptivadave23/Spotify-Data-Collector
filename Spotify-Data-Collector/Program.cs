using SpotifyAPI.Web;
using SpotifyUser;
using SpotifyDataCollector;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Carter;
using SpotifyAPI.Web.Http;
using Microsoft.Extensions.DependencyInjection;

DotEnv.Load();
var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Spotify>();
builder.Services.AddScoped<ISpotifyService, Spotify>();
builder.Services.AddScoped<ISpotifyService>(provider => new Spotify());
builder.Services.AddScoped<IUser, User>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(Options =>
{
    Options.Cookie.Name = "SpotifySession";
    Options.IdleTimeout = TimeSpan.FromMinutes(30);
    Options.Cookie.IsEssential = true;
    Options.Cookie.HttpOnly = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add Carter
builder.Services.AddCarter();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()   // Allow any origin
                   .AllowAnyMethod()   // Allow any HTTP method (GET, POST, etc.)
                   .AllowAnyHeader();  // Allow any HTTP headers
        });
});

var app = builder.Build();

// Use CORS only for development
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}

app.UseSession();

// Initialize the Spotify client
var serviceProvider = app.Services.CreateScope().ServiceProvider;
var spotifyService = serviceProvider.GetRequiredService<ISpotifyService>();
spotifyService.InitializeClientAsync().Wait();
var user = serviceProvider.GetRequiredService<IUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map Carter endpoints
app.MapCarter();

// Additional routes (make sure to add your other routes here)
app.MapGet("/", () =>
{
    Console.WriteLine("Route Hit");

    return Results.Ok("API is running!)");
})
.WithName("Test Route")
.WithSummary("This is a test route for the service.")
.WithDescription("This route is used to test the service and ensure that it is running correctly.")
.WithTags(new string[] { "Test", "Service" })
.WithDisplayName("Test Route");








app.Run();
