using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Transform _weaponSelectorItemParent;
    [SerializeField] List<Weapon> _weaponList;
    [SerializeField] Animator _animator;
    [SerializeField] private WeaponSelectorItem _weaponSelectorItemPrefab;

    private List<Team> _teams;
    private Worm _currentWorm;
    private bool _canOpen = false;

    public IReadOnlyList<Weapon> WeaponList => _weaponList;
    public Transform WeaponSelectorItemParent => _weaponSelectorItemParent;

    public Action<Worm> TurnStarted;

    public void Init(List<Weapon> weaponList)
    {
        _weaponList = weaponList;

        foreach (var weapon in _weaponList)
        {
            WeaponSelectorItem weaponItem = Instantiate(_weaponSelectorItemPrefab, _weaponSelectorItemParent);
            weaponItem.Init(this, weapon);
        }

        _game.WormsSpawned += OnWormsSpawned;
        _game.NextTurnStarted += OnNextTurnStarted;

        foreach (var weapon in _weaponList)
            weapon.Shot += OnWeaponShot;
    }

    private void OnDestroy()
    {
        _game.WormsSpawned -= OnWormsSpawned;
        _game.NextTurnStarted -= OnNextTurnStarted;

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

    private void OnProjectileExploded(Projectile projectile, Worm worm)
    {

    }

    private void OnNextTurnStarted()
    {
        Close();
    }
}
