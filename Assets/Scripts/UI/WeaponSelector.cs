using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Transform _container;
    [SerializeField] Weapon[] _weapons;
    [SerializeField] Animator _animator;

    private List<Team> _teams;
    private Worm _currentWorm;
    private bool _canOpen = false;
    private Weapon _currentWeapon;

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
        _currentWeapon = weapon;
        weapon.Shot += OnWeaponShot;
    }

    private void OnWeaponShot(Projectile projectile)
    {
        _currentWeapon.Shot -= OnWeaponShot;
        _currentWeapon = null;
        _canOpen = false;
    }
}
