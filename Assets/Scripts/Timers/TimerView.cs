using R3;
using TMPro;
using UnityEngine;

namespace Timers
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private ITimeFormatter _timeFormatter;

        private ReactiveTimer _reactiveTimer;

        public void Init(ReactiveTimer timer, ITimeFormatter timeFormatter)
        {
            _reactiveTimer = timer;
            _timeFormatter = timeFormatter;
            
            _reactiveTimer.TimeLeft.Subscribe(OnTimerUpdated);
        }
        
        private void OnTimerUpdated(double timeLeft) => _text.text = _timeFormatter.GetFormattedTime(timeLeft);
    }
}