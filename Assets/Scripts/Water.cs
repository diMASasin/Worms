using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private GlobalTimer _globalTimer;
    [SerializeField] private float _step;
    [SerializeField] private Game _game;

    private bool _isTimerOut = false;

    private void OnEnable()
    {
        _globalTimer.TimerOut += OnTimerOut;
        _game.TurnEnd += OnTurnEnd;
    }

    private void OnDisable()
    {
        _globalTimer.TimerOut -= OnTimerOut;
        _game.TurnEnd -= OnTurnEnd;
    }

    private void OnTimerOut()
    {
        _isTimerOut = true;
    }

    private void OnTurnEnd()
    {
        if(_isTimerOut)
            IncreaseLevel();
    }

    private void IncreaseLevel()
    {
        //transform.position += new Vector3(0, _step, 0);
        transform.DOMove(transform.position + new Vector3(0, 0 + _step, 0), 1);
    }
}
