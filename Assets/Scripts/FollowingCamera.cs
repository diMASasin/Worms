using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowingCamera : MonoBehaviour
{
    [SerializeField] private float _speed = 1;
    [SerializeField] private Camera _camera;
    [SerializeField] private int _minPosition = 2;
    [SerializeField] private int _maxPosition = 15;
    [SerializeField] private Vector2 _offset;

    private Game _game;
    private Transform _target;

    private IProjectileEvents _projectileEvents;
    private IWormEventsProvider _wormEvents;

    private Vector3 CameraPosition
    {
        get => _camera.transform.position;
        set => _camera.transform.position = value;
    }

    public void Init(Game game, IProjectileEvents projectileEvents, IWormEventsProvider wormEvents)
    {
        _wormEvents = wormEvents;
        _projectileEvents = projectileEvents;
        _game = game;
        
        _game.TurnStarted += OnTurnStarted;
        _wormEvents.WormDamageTook += OnDamageTook;
        _projectileEvents.ProjectileExploded += OnProjectileEvents;
        _projectileEvents.ProjectileLaunched += OnProjectileLaunched;
    }

    private void OnDestroy()
    {
        _game.TurnStarted -= OnTurnStarted;
        _wormEvents.WormDamageTook -= OnDamageTook;
        _projectileEvents.ProjectileExploded -= OnProjectileEvents;
        _projectileEvents.ProjectileLaunched -= OnProjectileLaunched;
    }

    private void Update()
    {
        TryZoom();
    }

    private void FixedUpdate()
    {
        if (_target != null)
            FollowTarget();
    }

    private void TryZoom()
    {
        if (Input.mouseScrollDelta.y < 0 && CameraPosition.z > _minPosition ||
            Input.mouseScrollDelta.y > 0 && CameraPosition.z < _maxPosition)
            CameraPosition += new Vector3(0, 0, Input.mouseScrollDelta.y);

        float newPositionZ = Mathf.Clamp(CameraPosition.z, _minPosition, _maxPosition);

        CameraPosition = new Vector3(CameraPosition.x, CameraPosition.y, newPositionZ);
    }

    private void FollowTarget()
    {
        Vector3 newPosition = _target.position + (Vector3)_offset;
        newPosition.z = transform.position.z;

        CameraPosition = Vector3.Lerp(CameraPosition, newPosition, _speed * Time.deltaTime);
    }

    private void OnProjectileLaunched(Projectile projectile, Vector2 velocity)
    {
        SetTarget(projectile.transform);
    }

    private void OnProjectileEvents(Projectile projectile)
    {
        SetTarget(_game.CurrentWorm.transform);
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
        if (_game.CurrentWorm != worm)
            SetTarget(worm.transform);
    }
}
