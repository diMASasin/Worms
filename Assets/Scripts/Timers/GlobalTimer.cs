using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimer : MonoBehaviour
{
    [SerializeField] private Timer _timer;
    [SerializeField] private float _time;

    public event Action TimerOut;
    public event Action<float> TimerUpdated;

    public float Time => _time;

    private void OnEnable()
    {
        _timer.TimerUpdated += OnTimerUpdated;
    }

    private void OnDisable()
    {
        _timer.TimerUpdated -= OnTimerUpdated;
    }

    private void Start()
    {
        _timer.StartTimer(_time, OnTimerOut);
    }

    private void OnTimerOut()
    {
        TimerOut?.Invoke();
    }

    private void OnTimerUpdated(float timeLeft)
    {
        TimerUpdated?.Invoke(timeLeft);
    }
}
