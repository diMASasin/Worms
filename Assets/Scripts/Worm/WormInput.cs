using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormInput : MonoBehaviour
{
    [SerializeField] private Worm _worm;
    [SerializeField] private Throwing _throwing;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _throwing.EnablePointerLine();
        }

        if(Input.GetMouseButton(0))
        {
            _throwing.SetPointerLinePositionAndScale();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _throwing.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _worm.Jump();
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        _worm.TryMove(horizontal);
    }
}
