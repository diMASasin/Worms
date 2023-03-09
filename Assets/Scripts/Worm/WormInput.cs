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
        if(Input.GetKey(KeyCode.W))
        {
            _throwing.RaiseScope();
        }

        if(Input.GetKey(KeyCode.S))
        {
            _throwing.LowerScope();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _throwing.EnablePointerLine();
        }

        if(Input.GetKey(KeyCode.Space))
        {
            _throwing.IncreaseShotPower();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _throwing.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _wormMovement.LongJump();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _wormMovement.HighJump();
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
        _throwing.Reset();
        InputEnabled?.Invoke();
    }

    public void DisableInput()
    {
        enabled = false;
        gameObject.layer = _defaultLayer;
        InputDisabled?.Invoke();
    }
}
