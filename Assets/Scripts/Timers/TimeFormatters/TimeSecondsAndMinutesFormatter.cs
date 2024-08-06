namespace Timers
{
    public class TimeSecondsAndMinutesFormatter : ITimeFormatter
    {
        public string GetFormattedTime(double timeLeft)
        {
            double minutes = timeLeft / 60;
            double seconds = timeLeft % 60;
            return $"{minutes:F0}:{seconds:00}";
        }
    }
}