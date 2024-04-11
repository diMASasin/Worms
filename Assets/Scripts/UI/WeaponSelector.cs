using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private WeaponSelectorItem _itemPrefab;
    [SerializeField] private Transform _itemParent;
    [SerializeField] Animator _animator;

    private List<Weapon> _weaponList;
    private Bootstrap _bootstrap;
    private Game _game;
    private List<Team> _teams;
    private Worm _currentWorm;
    private bool _canOpen = false;

    public IReadOnlyList<Weapon> WeaponList => _weaponList;

    public Action<Worm> TurnStarted;

    public void Init(List<Weapon> weaponList, Game game, Bootstrap bootstrap)
    {
        _weaponList = weaponList;
        _game = game;
        _bootstrap = bootstrap;

        foreach (var weapon in _weaponList)
        {
            WeaponSelectorItem weaponItem = Instantiate(_itemPrefab, _itemParent);
            weaponItem.Init(this, weapon);
        }

        _bootstrap.WormsSpawned += OnWormsSpawned;
        _game.TurnStarted += OnTurnStarted;

        foreach (var weapon in _weaponList)
            weapon.Shot += OnWeaponShot;
    }

    private void OnDestroy()
    {
        _bootstrap.WormsSpawned -= OnWormsSpawned;
        _game.TurnStarted -= OnTurnStarted;

        foreach (var weapon in _weaponList)
            weapon.Shot -= OnWeaponShot;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && _canOpen)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        _animator.SetBool("Opened", !_animator.GetBool("Opened"));
    }

    private void Close()
    {
        _animator.SetBool("Opened", false);
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
        TurnStarted?.Invoke(_currentWorm);
        _canOpen = true;
    }

    public void SelectWeapon(Weapon weapon)
    {
        _currentWorm.ChangeWeapon(weapon);
    }

    private void OnWeaponShot(Projectile projectile)
    {
        _canOpen = false;
    }

    private void OnTurnStarted()
    {
        Close();
    }
}
