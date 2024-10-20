using SpotifyAPI.Web;
using SpotifyUser;
using SpotifyDataCollector;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Carter;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Spotify>();  // Register the Spotify service
builder.Services.AddScoped<ISpotifyService, Spotify>(); // Register ISpotifyService
builder.Services.AddScoped<IUser, User>(); // Register IUser
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(Options =>
{
    Options.Cookie.Name = "SpotifySession";
    Options.IdleTimeout = TimeSpan.FromMinutes(30);
    Options.Cookie.IsEssential = true;
    Options.Cookie.HttpOnly = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Spotify API", Version = "v1" });
});

// Add Carter
builder.Services.AddCarter();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Use CORS only for development
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}

app.UseSession();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseStaticFiles();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spotify API v1");
    });
}

app.UseHttpsRedirection();

// Map Carter endpoints
app.MapCarter();

// Additional routes (make sure to add your other routes here)


app.Run();
