using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 1;
    [SerializeField] private Game _game;
    [SerializeField] private WeaponSelector _weaponSelector;

    private List<Worm> _worms = new List<Worm>();

    private void OnEnable()
    {
        _game.WormsSpawned += OnWormsSpawned;
        foreach (var weapon in _weaponSelector.Weapons)
        {
            weapon.Shot += OnShot;
            weapon.ProjectileExploded += OnProjectileExploded;
        }
    }

    private void OnDisable()
    {
        _game.WormsSpawned -= OnWormsSpawned;
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot -= OnShot;
    }

    private void FixedUpdate()
    {
        if (!_target)
            return;

        Vector3 newPosition = _target.position;
        newPosition.z = transform.position.z;

        transform.position = Vector3.Lerp(transform.position, newPosition, _speed * Time.deltaTime);
    }

    private void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnWormsSpawned(List<Team> teams)
    {
        foreach (var team in teams)
        {
            team.TurnStarted += OnTurnStarted;
            team.Died += OnTeamDied;
            _worms.AddRange(team.Worms);
        }

        foreach (var worm in _worms)
        {
            //worm.Weapon.Shot += OnShot;
            worm.Died += OnWormDied;
            worm.DamageTook += OnDamageTook;
            //worm.Weapon.ProjectileExploded += OnProjectileExploded;
        }
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        SetTarget(worm.transform);
    }

    private void OnTeamDied(Team team)
    {
        team.Died -= OnTeamDied;
        team.TurnStarted -= OnTurnStarted;
    }

    private void OnWormDied(Worm worm)
    {
        worm.Died -= OnWormDied;
        //worm.Weapon.Shot -= OnShot;
        worm.DamageTook -= OnDamageTook;
    }

    private void OnDamageTook(Worm worm)
    {
        SetTarget(worm.transform);
    }

    private void OnShot(Projectile bomb)
    {
        SetTarget(bomb.transform);
    }

    private void OnProjectileExploded(Projectile bomb, Worm worm)
    {
        worm.Weapon.ProjectileExploded -= OnProjectileExploded;
        SetTarget(worm.transform);
    }
}
