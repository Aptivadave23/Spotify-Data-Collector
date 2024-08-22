using System;
using SpotifyAPI.Web;

namespace SpotifyDataCollector
{
    public class Spotify
    {
        // Add your class members and methods here
        
        public void Connect()
        {
            // Implement Spotify connection logic here
        }
        
        public void Search(string query)
        {
            // Implement Spotify search logic here
        }
        
        public static async Task getCredentials()
        {
            var config = SpotifyClientConfig.CreateDefault();
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
var request = new ClientCredentialsRequest(clientId, clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);
            var spotify = new SpotifyClient(config.WithToken(response.AccessToken));

        }
        
    }
}