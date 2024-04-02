using TMPro;
using UnityEngine;

public class GlobalTimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private Timer _globalTimer;
    
    public void Init(Timer timer)
    {
        _globalTimer = timer;

        _globalTimer.TimerUpdated += OnTimerUpdated;
    }

    private void OnDestroy()
    {
        _globalTimer.TimerUpdated -= OnTimerUpdated;
    }

    private void OnTimerUpdated(float timeLeft)
    {
        float minutes = timeLeft / 60;
        float seconds = timeLeft % 60;
        _text.text = $"{minutes:F0}:{seconds:00}";
    }
}
