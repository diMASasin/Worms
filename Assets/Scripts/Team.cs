using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Team : MonoBehaviour
{
    private List<Worm> _worms = new List<Worm>();
    private Color _color;
    private int _currentWormIndex = -1;

    public IReadOnlyList<Worm> Worms => _worms;

    public event UnityAction<Team> Died;
    public event UnityAction<Worm, Team> TurnStarted;

    public void Init(List<Worm> worms, Color color)
    {
        _worms = worms;
        _color = color;

        for (int i = 0; i < _worms.Count; i++)
        {
            _worms[i].Init(color, $"Worm {i + 1}");
            _worms[i].Died += OnWormDied;
        }
    }

    public void StartTurn()
    {
        _currentWormIndex++;
        if(_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        Worm currentWorm = GetCurrentWorm();
        TurnStarted?.Invoke(currentWorm, this);
        currentWorm.WormInput.EnableInput();
    }

    public Worm GetCurrentWorm()
    {
        return _worms[_currentWormIndex];
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;
        _worms.Remove(worm);

        if(_worms.Count <= 0)
            Died?.Invoke(this);
    }
}
