using DG.Tweening;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private float _step;
    [SerializeField] private Game _game;

    private Timer _globalTimer;
    private bool _isTimerOut = false;

    public void Init(Timer globalTimer)
    {
        _globalTimer = globalTimer;

        _globalTimer.Elapsed += OnTimerOut;
        _game.TurnEnd += OnTurnEnd;
    }

    private void OnDestroy()
    {
        _globalTimer.Elapsed -= OnTimerOut;
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
        transform.DOMove(transform.position + new Vector3(0, 0 + _step, 0), 1);
    }
}
