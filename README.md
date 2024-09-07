# Spotify-Data-Collector
API to pull a user's Spotify information (recent songs played, playlists, etc) and save that information to a data store for reporting.  This is an extremely slow work in progress, as it is a side project taken on by a dad who doesn't have much free-time to write code.  Feel free to notify me of any issues you see with the service and I am open to any and all suggestions on what can be improved.

If you see something missing from the API, there's a good chance I haven't thought of it or haven't gotten around to working on it.

# To run application:
- You'll need to first create a developer account with Spotify and register your application.  Start here to do that:  https://developer.spotify.com/
- Once you have registered your account and app, you'll need to add a .env file to your local repo and store the following variables:
    - CLIENT_ID
    - CLIENT_SECRET
    - REDIRECT_URI
    - These values are referenced in code and can be found in the Spotify Developer console entry you created for your application.

- To register your Spotify account with the API, hit the ```/login``` route. You'll only need to hit this once to register your account with the API.  
- Routes that begin with ```/Spotify``` are spotify data routes, and do not require authentication.  Use these routes to search for artists, albums, songs, etc.
- Routes that begin with ```/user``` are user specific routes, and require authentication.  
    - currently the only user route is ```/user/recenttracks```, which will return the last 10 songs played by the authenticated user.


This API heavily utilizes JohnnyCrazy's SPotifyAPI-Net library (https://github.com/JohnnyCrazy/SpotifyAPI-NET).  
