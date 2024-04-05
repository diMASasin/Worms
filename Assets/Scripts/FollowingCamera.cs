using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Game _game;
    [SerializeField] private WeaponSelector _weaponSelector;
    [SerializeField] private Camera _camera;
    [SerializeField] private int _minPosition = 2;
    [SerializeField] private int _maxPosition = 15;
    [SerializeField] private Vector2 _offset;

    private Transform _target;

    private readonly List<Worm> _worms = new();

    public void Init()
    {
        _game.WormsSpawned += OnWormsSpawned;
        foreach (var weapon in _weaponSelector.WeaponList)
        {
            weapon.Shot += OnShot;
            weapon.ProjectileExploded += OnProjectileExploded;
        }
    }

    private void OnDestroy()
    {
        _game.WormsSpawned -= OnWormsSpawned;
        foreach (var weapon in _weaponSelector.WeaponList)
        {
            weapon.Shot -= OnShot;
            weapon.ProjectileExploded -= OnProjectileExploded;
        }
    }

    private void Update()
    {
        //if(Input.mouseScrollDelta.y > 0 && _camera.orthographicSize > _minSize || Input.mouseScrollDelta.y < 0 && _camera.orthographicSize < _maxSize)
        //{
        //    _camera.orthographicSize -= Input.mouseScrollDelta.y;
        //}
        if (Input.mouseScrollDelta.y < 0 && _camera.transform.position.z > _minPosition || Input.mouseScrollDelta.y > 0 && _camera.transform.position.z < _maxPosition)
            _camera.transform.position += new Vector3(0, 0, Input.mouseScrollDelta.y);

        if (_camera.transform.position.z < _minPosition)
            _camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, _minPosition);
        if (_camera.transform.position.z > _maxPosition)
            _camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, _maxPosition);
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
            worm.Died += OnWormDied;
            worm.DamageTook += OnDamageTook;
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
        worm.DamageTook -= OnDamageTook;
    }

    private void OnDamageTook(Worm worm)
    {
        if (_game.TryGetCurrentTeam().TryGetCurrentWorm() != worm)
            SetTarget(worm.transform);
    }

    private void OnShot(Projectile bomb)
    {
        SetTarget(bomb.transform);
    }

    private void OnProjectileExploded(Projectile projectile, Worm worm)
    {
        if(_game.TryGetCurrentTeam().TryGetCurrentWorm() != worm)
            SetTarget(worm.transform);
    }
}
