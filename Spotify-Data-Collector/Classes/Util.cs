namespace SpotifyDataCollector
{
    public class TimeZones: ITimeZone
    {
        /// <summary>
        /// Spotify API returns all times in UTC. This method converts the time to Central timezone.
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Date time string</returns>
        public string SetCentralTime(DateTime time)
        {
            //get Central timezone
            TimeZoneInfo centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            //convert time to Central timezone
            return TimeZoneInfo.ConvertTimeFromUtc(time, centralTimeZone).ToString("yyyy-MM-ddTHH:mm:ssZ");
        }
    }

}