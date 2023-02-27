using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwing : MonoBehaviour
{
    Vector3 mouseStart;
    [SerializeField] Renderer _renderer;
    [SerializeField] float _sencetivity = 0.01f;
    [SerializeField] float _speedMultiplier = 0.03f;
    [SerializeField] Bomb _bombPrefab;
    [SerializeField] Transform _pointerLine;
    [SerializeField] private Transform _spawnPoint;

    private void Start()
    {
        _renderer.enabled = false;
    }

    public void EnablePointerLine()
    {
        _renderer.enabled = true;
        mouseStart = Input.mousePosition;
    }

    public void SetPointerLinePositionAndScale()
    {
        Vector3 delta = Input.mousePosition - mouseStart;
        transform.right = delta;
        _pointerLine.localScale = new Vector3(delta.magnitude * _sencetivity, 1, 1);
    }

    public void Shoot()
    {
        Vector3 delta = Input.mousePosition - mouseStart;
        Vector3 velocity = delta * _speedMultiplier;

        _renderer.enabled = false;
        Bomb newBomb = Instantiate(_bombPrefab, _spawnPoint.position, Quaternion.identity);
        newBomb.SetVelocity(velocity);
    }
}
