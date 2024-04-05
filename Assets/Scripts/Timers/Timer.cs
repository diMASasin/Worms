using UnityEngine;
using UnityEngine.Events;

public class Timer
{
    private float _interval;
    private float _timeLeft;
    private bool _started = false;

    public event UnityAction<float> TimerUpdated;
    public event UnityAction Elapsed;

    private void Reset()
    {
        _timeLeft = _interval;
        _started = false;
    } 

    public void Start(float interval)
    {
        _interval = interval;
        Reset();
        TimerUpdated?.Invoke(_timeLeft);
        _started = true;
    }

    public void Stop()
    {
        _started = false;
    }

    public void Tick()
    {
        if (!_started) return;
        
        _timeLeft -= Time.deltaTime;

        if(_timeLeft <= 0)
        {
            _timeLeft = 0;
            Stop();

            Elapsed?.Invoke();
        }

        TimerUpdated?.Invoke(_timeLeft);
    }
}