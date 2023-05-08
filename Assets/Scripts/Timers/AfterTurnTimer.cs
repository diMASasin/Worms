using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterTurnTimer : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private TurnTimer _turnTimer;
    [SerializeField] private Timer _timer;
    [SerializeField] private Game _game;
    [SerializeField] private WeaponSelector _weaponSelector;

    private void OnEnable()
    {
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot += OnShot;
    }

    private void OnDisable()
    {
        foreach (var weapon in _weaponSelector.Weapons)
            weapon.Shot -= OnShot;
    }

    private void OnShot(Projectile projectile)
    {
        _timer.StartTimer(_time, OnTimerOut);
    }

    private void OnTimerOut()
    {
        Debug.Log("AfterTurn");
        _timer.StopTimer();
        _game.EndTurn();
        _game.StartNextTurnWithDelay(0);
    }
}
