using Microsoft.Extensions.Hosting;
using SpotifyDataCollector;
using System.Threading;
using System.Threading.Tasks;

public class SpotifyClientInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public SpotifyClientInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var spotifyService = scope.ServiceProvider.GetRequiredService<ISpotifyService>();
            await spotifyService.InitializeClientAsync(); // Ensure the client is initialized
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
