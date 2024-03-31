using UnityEngine;
using TMPro;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text  _timerText;  
    
    private Timer _timer;

    public void Init(Timer timer)
    {
        _timer = timer;

        _timer.TimerUpdated += OnTimerUpdated;
    }
    
    public void OnDestroy()
    {
        _timer.TimerUpdated -= OnTimerUpdated;
    }

    private void OnTimerUpdated(float timeLeft)
    {
         _timerText.text = ((int)timeLeft).ToString();
    }
}
