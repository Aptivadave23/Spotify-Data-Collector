using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity.Data;
using SpotifyDataCollector;
using SpotifyAPI.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace SpotifyUser{
    
    public class User: IUser{
        private string _loginURI = "http://localhost:5272/redirect";
        private ISpotifyService _spotifyService = new Spotify();

        public string LoginURI{
            get{
                return _loginURI;
            }
        }


        public Task InitiateSpotifyLoginAsync(HttpContext context)
        {
            var loginRequest = new SpotifyAPI.Web.LoginRequest(
                new Uri(_loginURI),
                _spotifyService.GetClientId(),
                SpotifyAPI.Web.LoginRequest.ResponseType.Code
            )
            {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserLibraryRead, Scopes.UserReadRecentlyPlayed, Scopes.UserTopRead }
            };
            var uri = loginRequest.ToUri();
            context.Response.Redirect(uri.ToString());
            return Task.CompletedTask;
        }

        public async Task<SpotifyClient> GetSpotifyClientAsync(HttpContext context)
        {
            var code = context.Request.Query["code"].ToString();
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(
                    _spotifyService.GetClientId(), // Replace with your Spotify Client ID
                    _spotifyService.GetClientSecret(), // Replace with your Spotify Client Secret
                    code,
                    new Uri(_loginURI)
                )
            );
            var spotify = new SpotifyClient(response.AccessToken);
            return spotify;
        }
        
    }
}