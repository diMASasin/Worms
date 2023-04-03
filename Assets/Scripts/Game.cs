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
    [SerializeField] private GameObject _endScreen;
    [SerializeField] private FollowingCamera _followingCamera;
    [SerializeField] private WeaponSelector _weaponSelector;

    private List<Team> _teams = new List<Team>();
    private List<Team> _currentTeams = new List<Team>();
    private int _currentTeamIndex = 0;

    public event UnityAction<List<Team>> WormsSpawned;

    private void OnEnable()
    {
        _wormsSpawner.WormSpawned += OnWormSpawned;
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.ProjectileExploded += OnProjectileExploded;
        
    }

    private void OnDisable()
    {
        _wormsSpawner.WormSpawned -= OnWormSpawned;
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

    private void OnProjectileExploded(Bomb bomb, Worm worm)
    {
        worm.RemoveWeaponWithDelay(_weaponSelector.Container);
        worm.WormInput.DisableInput();
        StartNextTurnWithDelay(_turnDelay);
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;
        worm.Weapon.ProjectileExploded -= OnProjectileExploded;

        if (worm.WormInput.enabled == true)
            StartNextTurnWithDelay(_turnDelay);
    }

    private void OnTeamDied(Team team)
    {
        _currentTeams.Remove(team);

        if (_currentTeams.Count <= 1)
            _endScreen.SetActive(true);
    }

    private void StartNextTurnWithDelay(float delay)
    {
        StartCoroutine(DelayedStartNextTurn(delay));
    }

    private IEnumerator DelayedStartNextTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_currentTeamIndex >= _currentTeams.Count)
            _currentTeamIndex = 0;

        _currentTeams[_currentTeamIndex].StartTurn();
        _currentTeamIndex++;
    }
}
