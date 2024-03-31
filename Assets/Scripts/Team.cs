using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Team : MonoBehaviour
{
    private List<Worm> _worms = new List<Worm>();
    private int _currentWormIndex = -1;

    public Color Color { get; set; }
    public string Name { get; set; }
    public int MaxHealth { get; set; }

    public IReadOnlyList<Worm> Worms => _worms;

    public event UnityAction<Team> Died;
    public event UnityAction<Worm, Team> TurnStarted;
    public event UnityAction<int> HealthChanged;

    public void Init(List<Worm> worms, Color color, string name)
    {
        _worms = worms;
        Color = color;
        Name = name;
        MaxHealth = 0;

        for (int i = 0; i < _worms.Count; i++)
        {
            MaxHealth += _worms[i].MaxHealth;
            _worms[i].Init(color, $"Worm {i + 1}");
            _worms[i].Died += OnWormDied;
            _worms[i].DamageTook += OnDamageTook;
        }
    }

    public void StartTurn()
    {
        _currentWormIndex++;
        if(_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        Worm currentWorm = TryGetCurrentWorm();
        TurnStarted?.Invoke(currentWorm, this);
        currentWorm.Input.EnableInput();
    }

    public Worm TryGetCurrentWorm()
    {
        if (_currentWormIndex >= _worms.Count)
            _currentWormIndex = 0;

        if (_worms.Count == 0)
            return null;

        return _worms[_currentWormIndex];
    }

    private void OnWormDied(Worm worm)
    {
        worm.DamageTook -= OnDamageTook;
        worm.Died -= OnWormDied;

        if (_currentWormIndex > _worms.IndexOf(worm))
            _currentWormIndex--;

        _worms.Remove(worm);

        OnDamageTook(worm);

        if(_worms.Count <= 0)
            Died?.Invoke(this);
    }

    private void OnDamageTook(Worm worm)
    {
        var sum = _worms.Sum(worm => worm.Health);
        HealthChanged?.Invoke(sum);
    }
}
