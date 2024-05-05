using System;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : IDisposable
{
    public bool IsEnabled { get; private set; } = false;

    private MainInput _input;
    private readonly Game _game;
    private Worm _worm;
    private Weapon _weapon;

    private Movement _movement => _worm.Movement;
    private WeaponView _weaponView => _worm.WeaponView;

    public event UnityAction InputEnabled;
    public event UnityAction InputDisabled;

    public PlayerInput(Game game)
    {
        _game = game;

        _input = new MainInput();

        _game.TurnStarted += OnTurnStarted;
        _game.TurnEnd += OnTurnEnd;
        
        _input.Main.TurnRight.performed += OnTurnRight;
        _input.Main.TurnLeft.performed += OnTurnLeft;
        _input.Main.LongJump.performed += OnLongJump;
        _input.Main.HighJump.performed += OnHighJump;
        _input.Main.EnablePointerLine.performed += OnEnablePointerLine;
        _input.Main.Shoot.performed += OnShoot;
    }


    public void Dispose()
    {
        _game.TurnStarted -= OnTurnStarted;
        _game.TurnEnd -= OnTurnEnd;

        _worm.WeaponChanged -= OnWeaponChanged;
        _worm.DamageTook -= OnDamageTook;
        
        _input.Main.TurnRight.performed -= OnTurnRight;
        _input.Main.TurnLeft.performed -= OnTurnLeft;
        _input.Main.LongJump.performed -= OnLongJump;
        _input.Main.HighJump.performed -= OnHighJump;
        _input.Main.EnablePointerLine.performed -= OnEnablePointerLine;
        _input.Main.Shoot.performed -= OnShoot;
    }

    public void Tick()
    {
        bool canMove = _input.Main.DontMove.ReadValue<float>() == 0;
        if (canMove == true)
            OnDirectionChanged();

        OnAimDirectionChanged();
        OnIncreaseShotPower();
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        ChangeWorm(worm, team);
        EnableInput();
    }

    private void OnTurnEnd()
    {
        if(_worm == null)
            return;
        CoroutinePerformer.StartRoutine(_worm.SetRigidbodyKinematicWhenGrounded());
        DisableInput();
    }

    private void EnableInput()
    {
        _input.Enable();
        IsEnabled = true;
        _worm.SetRigidbodyDynamic();
        InputEnabled?.Invoke();
    }

    public void DisableInput()
    {
        _input.Disable();
        IsEnabled = false;
        InputDisabled?.Invoke();
        _movement.Reset();
    }

    private void ChangeWorm(Worm newWorm, Team team)
    {
        if (_worm != null)
        {
            _worm.WeaponChanged -= OnWeaponChanged;
            _worm.DamageTook -= OnDamageTook;
        }

        _worm = newWorm;

        if (_worm != null)
        {
            _worm.WeaponChanged += OnWeaponChanged;
            _worm.DamageTook += OnDamageTook;
        }
    }

    private void OnWeaponChanged(Weapon weapon)
    {
        _weapon = weapon;
    }

    private void OnShoot(CallbackContext obj)
    {
        _weapon.Shoot();
    }

    private void OnEnablePointerLine(CallbackContext obj)
    {
        if (_weapon == null)
            return;

        _weapon.StartIncresePower();
    }

    private void OnHighJump(CallbackContext obj)
    {
        _movement.HighJump();
    }

    private void OnLongJump(CallbackContext obj)
    {
        _movement.LongJump();
    }

    private void OnTurnRight(CallbackContext obj)
    {
        _movement.TurnRight();
    }

    private void OnTurnLeft(CallbackContext obj)
    {
        _movement.TurnLeft();
    }

    private void OnDamageTook(Worm worm)
    {
        if(IsEnabled == true)
            DisableInput();
    }

    private void OnAimDirectionChanged()
    {
        if( _weapon == null )
            return;

        var direction = _input.Main.RaiseScope.ReadValue<float>();
        _weapon.MoveScope(direction);
    }

    private void OnDirectionChanged()
    {
        if(_worm == null)
            return;

        var direction = _input.Main.Move.ReadValue<float>();
        _movement.TryMove(direction);
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
