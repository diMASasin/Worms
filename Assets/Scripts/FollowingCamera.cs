using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Camera _camera;
    [SerializeField] private int _minPosition = 2;
    [SerializeField] private int _maxPosition = 15;
    [SerializeField] private Vector2 _offset;

    private WeaponSelector _weaponSelector;
    private Game _game;
    private Transform _target;

    private IReadOnlyList<Team> _teams;
    private IReadOnlyList<Worm> _worms;

    private Vector3 CameraPosition
    {
        get => _camera.transform.position;
        set => _camera.transform.position = value;
    }

    public void Init(Game game, WeaponSelector weaponSelector, IReadOnlyList<Team> teams, IReadOnlyList<Worm> worms)
    {
        _game = game;
        _teams = teams;
        _worms = worms;
        _weaponSelector = weaponSelector;

        foreach (var weapon in _weaponSelector.WeaponList)
        {
            weapon.Shot += OnShot;
            weapon.ProjectileExploded += OnProjectileExploded;
        }

        foreach (var team in _teams)
            team.TurnStarted += OnTurnStarted;

        foreach (var worm in _worms)
            worm.DamageTook += OnDamageTook;
    }

    private void OnDestroy()
    {
        foreach (var weapon in _weaponSelector.WeaponList)
        {
            weapon.Shot -= OnShot;
            weapon.ProjectileExploded -= OnProjectileExploded;
        }

        foreach (var team in _teams)
            team.TurnStarted += OnTurnStarted;

        foreach (var worm in _worms)
            worm.DamageTook += OnDamageTook;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y < 0 && CameraPosition.z > _minPosition || Input.mouseScrollDelta.y > 0 && CameraPosition.z < _maxPosition)
            CameraPosition += new Vector3(0, 0, Input.mouseScrollDelta.y);

        float newPositionZ = Mathf.Clamp(CameraPosition.z, _minPosition, _maxPosition);

        CameraPosition = new Vector3(CameraPosition.x, CameraPosition.y, newPositionZ);
    } 

    private void FixedUpdate()
    {
        if (!_target)
            return;

        Vector3 newPosition = _target.position + (Vector3)_offset;
        newPosition.z = transform.position.z;

        CameraPosition = Vector3.Lerp(CameraPosition, newPosition, _speed * Time.deltaTime);
    }

    private void SetTarget(Transform target)
    {
        _target = target;
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        SetTarget(worm.transform);
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
