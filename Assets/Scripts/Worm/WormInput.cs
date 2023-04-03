using UnityEngine;
using UnityEngine.Events;

public class WormInput : MonoBehaviour
{
    [SerializeField] private WormMovement _wormMovement;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private int _defaultLayer = 0;
    [SerializeField] private int _currentWormLayer = 6;
    [SerializeField] private Arrow _arrow;

    public event UnityAction InputEnabled;
    public event UnityAction InputDisabled;

    private void Update()
    {
        if(_weapon && Input.GetKey(KeyCode.W))
        {
            _weapon.RaiseScope();
        }

        if(_weapon && Input.GetKey(KeyCode.S))
        {
            _weapon.LowerScope();
        }

        if (_weapon && Input.GetKeyDown(KeyCode.Space))
        {
            _weapon.EnablePointerLine();
        }

        if(_weapon && Input.GetKey(KeyCode.Space))
        {
            _weapon.IncreaseShotPower();
        }

        if (_weapon && Input.GetKeyUp(KeyCode.Space))
        {
            _weapon.Shoot();
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
        InputEnabled?.Invoke();
    }

    public void DisableInput()
    {
        enabled = false;
        gameObject.layer = _defaultLayer;
        InputDisabled?.Invoke();
    }

    public void ChangeWeapon(Weapon weapon)
    {
        _weapon = weapon;
    }
}
