using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalTimerView : MonoBehaviour
{
    [SerializeField] private GlobalTimer _globalTimer;
    [SerializeField] private TMP_Text _text;

    private void OnEnable()
    {
        _globalTimer.TimerUpdated += OnTimerUpdated;
    }

    private void OnDisable()
    {
        _globalTimer.TimerUpdated -= OnTimerUpdated;
    }

    private void OnTimerUpdated(float timeLeft)
    {
        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        _text.text = $"{minutes}:{seconds}";
    }
}
