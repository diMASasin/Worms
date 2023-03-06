using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Team : MonoBehaviour
{
    private List<Worm> _worms = new List<Worm>();
    private Color _color;
    private int _currentWormIndex = 0;

    public event UnityAction<Team> Died;

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
        if(_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        _worms[_currentWormIndex].WormInput.EnableInput();
        _currentWormIndex++;
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;
        _worms.Remove(worm);

        if(_worms.Count <= 0)
            Died?.Invoke(this);
    }
}
