using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text  _timerText;  
    [SerializeField] private Timer _timer;

    public void OnEnable()
    {
        _timer.TimerUpdated += OnTimerUpdated;
    }

    public void OnDisable()
    {
        _timer.TimerUpdated -= OnTimerUpdated;
    }

    private void OnTimerUpdated(float timeLeft)
    {
         _timerText.text = ((int)timeLeft).ToString();
    }
}
