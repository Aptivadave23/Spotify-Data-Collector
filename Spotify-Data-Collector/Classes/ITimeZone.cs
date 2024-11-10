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

        /// <summary>
        /// Convert the time to Unix milliseconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        long ConvertToUnixMilliseconds(DateTime time);
    }
}