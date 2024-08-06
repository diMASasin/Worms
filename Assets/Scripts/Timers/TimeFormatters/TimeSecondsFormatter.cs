namespace Timers
{
    public class TimeSecondsFormatter : ITimeFormatter
    {
        public string GetFormattedTime(double timeLeft) => $"{timeLeft:F0}";
    }
}