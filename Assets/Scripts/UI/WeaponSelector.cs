using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Transform _container;
    [SerializeField] Weapon[] _weapons;

    private List<Team> _teams;
    private Worm _currentWorm;

    public IReadOnlyCollection<Weapon> Weapons => _weapons;
    public Transform Container => _container;

    private void OnEnable()
    {
        _game.WormsSpawned += OnWormsSpawned;
    }

    private void OnDisable()
    {
        _game.WormsSpawned -= OnWormsSpawned;
    }

    private void OnWormsSpawned(List<Team> teams)
    {
        _teams = teams;
        foreach (var team in _teams)
        {
            team.TurnStarted += OnTurnStarted;
            team.Died += OnTeamDied;
        }
    }

    private void OnTeamDied(Team team)
    {
        team.TurnStarted -= OnTurnStarted;
        team.Died -= OnTeamDied;
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        _currentWorm = worm;
    }

    public void SelectWeapon(Weapon weapon)
    {
        _currentWorm.ChangeWeapon(weapon, _container);
    }
}
