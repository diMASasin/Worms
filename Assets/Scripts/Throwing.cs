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

    private void Start()
    {
        _renderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _renderer.enabled = true;
            mouseStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - mouseStart;
            transform.right = delta;
            _pointerLine.localScale = new Vector3(delta.magnitude * _sencetivity, 1, 1);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 delta = Input.mousePosition - mouseStart;
            Vector3 velocity = delta * _speedMultiplier;

            _renderer.enabled = false;
            Bomb newBomb = Instantiate(_bombPrefab, transform.position, Quaternion.identity);
            newBomb.SetVelocity(velocity);
        }
    }
}
