using UnityEngine;
using UnityEngine.Events;

public class WormInput : MonoBehaviour
{
    [SerializeField] private WormMovement _wormMovement;
    [SerializeField] private Throwing _throwing;
    [SerializeField] private int _defaultLayer = 0;
    [SerializeField] private int _currentWormLayer = 6;
    [SerializeField] private Arrow _arrow;

    public event UnityAction InputEnabled;
    public event UnityAction InputDisabled;

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
            _wormMovement.Jump();
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        _wormMovement.TryMove(horizontal);
    }

    public void EnableInput()
    {
        enabled = true;
        gameObject.layer = _currentWormLayer;
        _arrow.StartMove();
        InputEnabled?.Invoke();
    }

    public void DisableInput()
    {
        enabled = false;
        gameObject.layer = _defaultLayer;
        InputDisabled?.Invoke();
    }
}
