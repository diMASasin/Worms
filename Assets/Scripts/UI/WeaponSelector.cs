using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    [SerializeField] private WeaponSelectorItem _itemPrefab;
    [SerializeField] private Transform _itemParent;
    [SerializeField] Animator _animator;

    private List<Weapon> _weaponList;
    private Game _game;
    private bool _canOpen = false;
    private List<WeaponSelectorItem> _items = new();

    public IReadOnlyList<Weapon> WeaponList => _weaponList;

    public Action<Worm> TurnStarted;

    public void Init(List<Weapon> weaponList, Game game)
    {
        _weaponList = weaponList;
        _game = game;

        foreach (var weapon in _weaponList)
        {
            WeaponSelectorItem weaponItem = Instantiate(_itemPrefab, _itemParent);
            weaponItem.Selected += OnSelected;
            weaponItem.Init(weapon);
        }

        _game.TurnStarted += OnTurnStarted;

        foreach (var weapon in _weaponList)
            weapon.Shot += OnWeaponShot;
    }


    private void OnDestroy()
    {
        if(_game != null)
            _game.TurnStarted -= OnTurnStarted;

        if(_weaponList != null)
            foreach (var weapon in _weaponList)
                weapon.Shot -= OnWeaponShot;

        if(_items != null)
            foreach (var item in _items)
                item.Selected -= OnSelected;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && _canOpen)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        _animator.SetBool("Opened", !_animator.GetBool("Opened"));
    }

    private void OnSelected(Weapon weapon)
    {
        _game.CurrentWorm.ChangeWeapon(weapon);
        Close();
    }

    private void Close()
    {
        _animator.SetBool("Opened", false);
    }

    private void OnTurnStarted(Worm worm, Team team)
    {
        Close();
        _canOpen = true;
    }

    private void OnWeaponShot(Projectile projectile)
    {
        _canOpen = false;
    }
}
