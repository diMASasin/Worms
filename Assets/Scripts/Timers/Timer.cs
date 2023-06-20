using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private float _timeStart;
    private float _timeLeft;
    private bool _started = false;

    public event UnityAction<float> TimerUpdated;
    private event UnityAction OnTimerOut;

    private void Reset()
    {
        _timeLeft = _timeStart;
        _started = false;
    } 

    public void StartTimer(float time, UnityAction onTimerOut)
    {
        _timeStart = time;
        Reset();
        TimerUpdated?.Invoke(_timeLeft);
        OnTimerOut = onTimerOut;
        _started = true;
    }

    public void StopTimer()
    {
        _started = false;
    }

    private void Update()
    {
        if (_started) 
        {
            _timeLeft -= Time.deltaTime;

            if(_timeLeft <= 0)
            {
                _timeLeft = 0;
                StopTimer();
                OnTimerOut();
            }

            TimerUpdated?.Invoke(_timeLeft);
        }
    }
}