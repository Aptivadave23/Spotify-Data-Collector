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

        /// <summary>
        /// Convert the time to Unix milliseconds
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long ConvertToUnixMilliseconds(DateTime time)
        {
            // Check if the time's Kind is set correctly
            if (time.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("DateTime Kind should be either UTC or Local", nameof(time));
            }
            
            // Convert to UTC
            DateTime utcTime = time.ToUniversalTime();
            
            // Define the Unix epoch start time
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // Check if time is before epoch to avoid overflow
            if (utcTime < epoch)
            {
                throw new ArgumentOutOfRangeException(nameof(time), "DateTime is before the Unix epoch.");
            }
            
            // Calculate and return the time difference in milliseconds
            return (long)(utcTime - epoch).TotalMilliseconds;
        }

    }

}