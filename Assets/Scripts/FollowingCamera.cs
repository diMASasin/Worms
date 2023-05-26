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
    [SerializeField] private Camera _camera;
    [SerializeField] private int _minSize = 2;
    [SerializeField] private int _maxSize = 15;
    [SerializeField] private Vector2 _offset;

    private List<Worm> _worms = new List<Worm>();

    private void OnValidate()
    {
        _camera = GetComponent<Camera>();
        _weaponSelector = FindObjectOfType<WeaponSelector>();
    }

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
        {
            weapon.Shot -= OnShot;
            weapon.ProjectileExploded -= OnProjectileExploded;
        }
    }

    private void Update()
    {
        if(Input.mouseScrollDelta.y > 0 && _camera.orthographicSize > _minSize || Input.mouseScrollDelta.y < 0 && _camera.orthographicSize < _maxSize)
        {
            _camera.orthographicSize -= Input.mouseScrollDelta.y;
        }
    }

    private void FixedUpdate()
    {
        if (!_target)
            return;

        Vector3 newPosition = _target.position + (Vector3)_offset;
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
        if (_game.GetCurrentTeam().GetCurrentWorm() == worm)
            SetTarget(worm.transform);
    }

    private void OnShot(Projectile bomb)
    {
        SetTarget(bomb.transform);
    }

    private void OnProjectileExploded(Projectile bomb, Worm worm)
    {
        if(_game.GetCurrentTeam().GetCurrentWorm() == worm)
            SetTarget(worm.transform);
    }
}
