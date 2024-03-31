using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Transform _container;
    [SerializeField] List<Weapon> _weaponArray;
    [SerializeField] Animator _animator;

    private List<Team> _teams;
    private Worm _currentWorm;
    private bool _canOpen = false;

    public IReadOnlyList<Weapon> WeaponArray => _weaponArray;
    public Transform Container => _container;

    private void OnValidate()
    {
        _game = FindObjectOfType<Game>();
    }

    public void Init(List<Weapon> weaponArray)
    {
        _weaponArray = weaponArray;

        _game.WormsSpawned += OnWormsSpawned;
        _game.NextTurnStarted += OnNextTurnStarted;

        foreach (var weapon in _weaponArray)
            weapon.Shot += OnWeaponShot;
    }

    private void OnDestroy()
    {
        _game.WormsSpawned -= OnWormsSpawned;
        _game.NextTurnStarted -= OnNextTurnStarted;

        foreach (var weapon in _weaponArray)
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
        _canOpen = true;
    }

    public void SelectWeapon(Weapon weapon)
    {
        _currentWorm.ChangeWeapon(weapon, _container);
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
