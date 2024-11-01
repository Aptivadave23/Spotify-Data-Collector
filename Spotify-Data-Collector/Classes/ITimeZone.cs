namespace SpotifyDataCollector
{
    public interface ITimeZone
    {
         /// <summary>
        /// Spotify API returns all times in UTC. This method converts the time to Central timezone.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Date time string</returns>
        string SetCentralTime(DateTime time);
    }
}