using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Throwing : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _sencetivity = 0.01f;
    [SerializeField] private float _scopeSencetivity = 0.7f;
    [SerializeField] private float _speedMultiplier = 0.03f;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Worm _worm;
    [SerializeField] private float _shotPower = 5;
    [SerializeField] private float _maxShotPower = 5;

    private Vector3 _mouseStart;
    private float _currentShotPower = 0;
    private bool _shot = false;

    public event UnityAction<Bomb, Worm> ProjectileExploded;
    public event UnityAction<Bomb> Shot;

    private void Start()
    {
        _renderer.enabled = false;
    }

    public void Reset()
    {
        _shot = false;
    }

    public void EnablePointerLine()
    {
        if (_shot)
            return;

        _renderer.enabled = true;
        _mouseStart = Input.mousePosition;
    }

    public void RaiseScope()
    {
        transform.Rotate(new Vector3(0, 0, -0.7f));
    }

    public void LowerScope()
    {
        transform.Rotate(new Vector3(0, 0, 0.7f));
    }

    public void IncreaseShotPower()
    {
        if (_currentShotPower >= _maxShotPower || _shot)
            return;

        _currentShotPower += _shotPower * Time.deltaTime;
        _pointerLine.localScale = new Vector3(_currentShotPower / _maxShotPower, 1, 1);

        if (_currentShotPower >= _maxShotPower)
        {
            _currentShotPower = _maxShotPower;
            Shoot();
        }
    }

    public void Shoot()
    {
        if (_shot)
            return;

        Vector3 delta = Input.mousePosition - _mouseStart;
        Vector3 velocity = _currentShotPower * transform.right;

        _renderer.enabled = false;
        Bomb newBomb = Instantiate(_bombPrefab, _spawnPoint.position, Quaternion.identity);
        Shot?.Invoke(newBomb);
        _shot = true;
        newBomb.Exploded += OnProjectileExploded;
        newBomb.SetVelocity(velocity);
    }

    private void OnProjectileExploded(Bomb bomb)
    {
        bomb.Exploded -= OnProjectileExploded;
        ProjectileExploded?.Invoke(bomb, _worm);
    }
}
