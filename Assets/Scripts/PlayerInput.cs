using System;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : IDisposable
{
    public bool IsEnabled { get; private set; } = false;

    private WormMovement _wormMovement;
    private Worm _worm;
    private readonly WeaponView _weaponView;
    private MainInput _input;
    private Weapon _weapon;

    public event UnityAction InputEnabled;
    public event UnityAction InputDisabled;

    public PlayerInput(WormMovement wormMovement, Worm worm, WeaponView weaponView)
    {
        _wormMovement = wormMovement;
        _worm = worm;
        _weaponView = weaponView;

        _input = new MainInput();

        _worm.WeaponChanged += OnWeaponChanged;
        _input.Main.TurnRight.performed += OnTurnRight;
        _input.Main.TurnLeft.performed += OnTurnLeft;
        _input.Main.LongJump.performed += OnLongJump;
        _input.Main.HighJump.performed += OnHighJump;
        _input.Main.EnablePointerLine.performed += OnEnablePointerLine;
        _input.Main.Shoot.performed += OnShoot;
    }

    public void Dispose()
    {
        _worm.WeaponChanged -= OnWeaponChanged;
        _input.Main.TurnRight.performed -= OnTurnRight;
        _input.Main.TurnLeft.performed -= OnTurnLeft;
        _input.Main.LongJump.performed -= OnLongJump;
        _input.Main.HighJump.performed -= OnHighJump;
        _input.Main.EnablePointerLine.performed -= OnEnablePointerLine;
        _input.Main.Shoot.performed -= OnShoot;
    }

    public void Tick()
    {
        var canMove = _input.Main.DontMove.ReadValue<float>();
        if (canMove == 0)
            OnDirectionChanged();

        OnAimDirectionChanged();
        OnIncreaseShotPower();
    }

    public void EnableInput()
    {
        _input.Enable();
        IsEnabled = true;
        InputEnabled?.Invoke();
    }

    public void DisableInput()
    {
        _input.Disable();
        IsEnabled = false;
        InputDisabled?.Invoke();
        _wormMovement.Reset();
    }

    private void OnShoot(CallbackContext obj)
    {
        _weapon.Shoot();
    }

    private void OnEnablePointerLine(CallbackContext obj)
    {
        if (_weapon == null)
            return;

        _weapon.EnablePointerLine();
    }

    private void OnHighJump(CallbackContext obj)
    {
        _wormMovement.HighJump();
    }

    private void OnLongJump(CallbackContext obj)
    {
        _wormMovement.LongJump();
    }

    private void OnTurnRight(CallbackContext obj)
    {
        _wormMovement.TurnRight();
    }

    private void OnTurnLeft(CallbackContext obj)
    {
        _wormMovement.TurnLeft();
    }

    private void OnWeaponChanged(Weapon weapon)
    {
        _weapon = weapon;
    }

    private void OnAimDirectionChanged()
    {
        if( _weapon == null )
            return;

        var direction = _input.Main.RaiseScope.ReadValue<float>();
        _weaponView.MoveScope(direction);
    }

    private void OnDirectionChanged()
    {
        var direction = _input.Main.Move.ReadValue<float>();
        _wormMovement.TryMove(direction);
    }

    private void OnIncreaseShotPower()
    {
        if (_weapon == null)
            return;

        var isIncreasing = _input.Main.IncreaseShotPower.ReadValue<float>();
        if(isIncreasing == 1)
            _weapon.IncreaseShotPower();
    }
}
