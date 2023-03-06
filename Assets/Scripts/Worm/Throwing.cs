using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Throwing : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _sencetivity = 0.01f;
    [SerializeField] private float _speedMultiplier = 0.03f;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Worm _worm;

    private Vector3 _mouseStart;

    public event UnityAction<Worm> ProjectileExploded;

    private void Start()
    {
        _renderer.enabled = false;
    }

    public void EnablePointerLine()
    {
        _renderer.enabled = true;
        _mouseStart = Input.mousePosition;
    }

    public void SetPointerLinePositionAndScale()
    {
        Vector3 delta = Input.mousePosition - _mouseStart;
        transform.right = delta;
        _pointerLine.localScale = new Vector3(delta.magnitude * _sencetivity, 1, 1);
    }

    public void Shoot()
    {
        Vector3 delta = Input.mousePosition - _mouseStart;
        Vector3 velocity = delta * _speedMultiplier;

        _renderer.enabled = false;
        Bomb newBomb = Instantiate(_bombPrefab, _spawnPoint.position, Quaternion.identity);
        newBomb.Exploded += OnProjectileExploded;
        newBomb.SetVelocity(velocity);
    }

    private void OnProjectileExploded(Bomb bomb)
    {
        bomb.Exploded -= OnProjectileExploded;
        ProjectileExploded?.Invoke(_worm);
    }
}
