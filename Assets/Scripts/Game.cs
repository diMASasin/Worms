using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    [SerializeField] private WormsSpawner _wormsSpawner;
    [SerializeField] private float _turnDelay = 2.5f;
    [SerializeField] private EndScreen _endScreen;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private WeaponSelector _weaponSelector;

    private List<Team> _teams = new List<Team>();
    private List<Team> _currentTeams = new List<Team>();
    private int _currentTeamIndex = -1;

    public event UnityAction<List<Team>> WormsSpawned;

    private void OnValidate()
    {
        _weaponSelector = FindObjectOfType<WeaponSelector>();
        _endScreen = FindObjectOfType<EndScreen>();
    }

    private void OnEnable()
    {
        _wormsSpawner.WormSpawned += OnWormSpawned;
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.ProjectileExploded += OnProjectileExploded;
    }

    private void OnDisable()
    {
        _wormsSpawner.WormSpawned -= OnWormSpawned;
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.ProjectileExploded -= OnProjectileExploded;
    }

    private void Start()
    {
        _wormsSpawner.GetEdgesForSpawn();
        _teams = _wormsSpawner.SpawnTeams();
        WormsSpawned?.Invoke(_teams);

        foreach (var team in _teams)
        {
            team.Died += OnTeamDied;
        }

        _currentTeams.AddRange(_teams);

        StartNextTurnWithDelay(0);
    }

    private void OnWormSpawned(Worm worm)
    {
        worm.Died += OnWormDied;
        //worm.Weapon.ProjectileExploded += OnProjectileExploded;
    }

    private void OnProjectileExploded(Projectile bomb, Worm worm)
    {
        //EndTurn();
        //StartNextTurnWithDelay(_turnDelay);
    }

    public Team GetCurrentTeam()
    {
        return _currentTeams[_currentTeamIndex];
    }

    public void EndTurn()
    {
        var currentWorm = _currentTeams[_currentTeamIndex].GetCurrentWorm();

        currentWorm.RemoveWeaponWithDelay(_weaponSelector.Container);
        currentWorm.WormInput.DisableInput();
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;

        worm.TryRemoveWeapon(_weaponSelector.Container);

        if (worm.WormInput.enabled == true)
            StartNextTurnWithDelay(_turnDelay);
    }

    private void OnTeamDied(Team team)
    {
        _currentTeams.Remove(team);

        if (_currentTeams.Count <= 1)
            _endScreen.gameObject.SetActive(true);
    }

    public IEnumerator WaitUntilProjectilesExplode(Action action)
    {
        while (_weaponSelector.ProjectilesCount > 0)
            yield return null;

        action();
    }

    public void StartNextTurnWithDelay(float delay)
    {
        StartCoroutine(DelayedStartNextTurn(delay));
    }

    private IEnumerator DelayedStartNextTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        _currentTeamIndex++;
        if (_currentTeamIndex >= _currentTeams.Count)
            _currentTeamIndex = 0;

        _currentTeams[_currentTeamIndex].StartTurn();
    }
}
