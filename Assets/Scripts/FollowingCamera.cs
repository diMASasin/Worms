using System.Collections.Generic;
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
    private ProjectileViewPool _viewPool;

    private IReadOnlyList<Worm> _worms;

    private Vector3 CameraPosition
    {
        get => _camera.transform.position;
        set => _camera.transform.position = value;
    }

    public void Init(Game game, IReadOnlyList<Worm> worms, ProjectileViewPool viewPool)
    {
        _game = game;
        _worms = worms;
        _viewPool = viewPool;

        foreach (var worm in _worms)
            worm.DamageTook += OnDamageTook;

        _game.TurnStarted += OnTurnStarted;
        _viewPool.Got += OnProjectileLaunched;
        _viewPool.Released += OnProjectileExploded;
    }

    private void OnDestroy()
    {
        foreach (var worm in _worms)
            worm.DamageTook -= OnDamageTook;

        _game.TurnStarted -= OnTurnStarted;
        _viewPool.Got -= OnProjectileLaunched;
        _viewPool.Released -= OnProjectileExploded;
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

    private void OnProjectileLaunched(ProjectileView projectileView)
    {
        SetTarget(projectileView.transform);
    }

    private void OnProjectileExploded(ProjectileView view)
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
