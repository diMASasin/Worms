using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    
    [SerializeField] private float _timeStart = 60;
    
    private float _timeLeft;
    private bool _started = false;
    public event UnityAction<float> TimerUpdated;

    public void StartTimer()
    {
        Reset();
        TimerUpdated?.Invoke(_timeLeft);
        _started = true;
    }

    private void Reset()
    {
        _timeLeft = _timeStart;
        _started = false;
    } 

    private void Update()
    {
        if (_started) 
        {
            _timeLeft -= Time.deltaTime;
           TimerUpdated?.Invoke(_timeLeft);
        }
    }
}