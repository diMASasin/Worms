using TMPro;
using UnityEngine;

namespace Timers
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        TimerFormattingStyle _formattingStyle;
        private Timer _timer;
    
        public void Init(Timer timer, TimerFormattingStyle style)
        {
            _timer = timer;
            _formattingStyle = style;

            _timer.TimerUpdated += OnTimerUpdated;
        }

        private void OnDestroy()
        {
            if(_timer != null)
                _timer.TimerUpdated -= OnTimerUpdated;
        }

        private void OnTimerUpdated(float timeLeft)
        {
            switch (_formattingStyle)
            {
                case TimerFormattingStyle.Seconds:
                    _text.text = $"{timeLeft:F0}";
                    break;

                case TimerFormattingStyle.MinutesAndSeconds:
                    float minutes = timeLeft / 60;
                    float seconds = timeLeft % 60;
                    _text.text = $"{minutes:F0}:{seconds:00}";
                    break;
            }
        }
    }

    public enum TimerFormattingStyle
    {
        Seconds,
        MinutesAndSeconds
    }
}