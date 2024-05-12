using System;
using System.Collections.Generic;
using EventProviders;
using Factories;
using UnityEngine;

public class WeaponSelector : MonoBehaviour, IWeaponSelectedEventProvider
{
    [SerializeField] Animator _animator;
    [SerializeField] private WeaponSelectorItemFactory _itemFactory;
    [SerializeField] private Transform _itemContainer;
    
    private bool _canOpen = false;
    private IWeaponShotEventProvider _shotEvent;
    private IGameEventsProvider _gameEvents;
    private IWeaponSelectedEventProvider _selectedEvent;
    private List<Weapon> _weaponList;

    public event Action<Weapon> WeaponSelected;

    public void Init(List<Weapon> weaponList, IWeaponShotEventProvider shotEvent,
        IGameEventsProvider gameEvents)
    {
        _weaponList = weaponList;
        _gameEvents = gameEvents;
        _shotEvent = shotEvent;
        _selectedEvent = _itemFactory;
        
        _itemFactory.Create(weaponList, _itemContainer);

        _gameEvents.TurnStarted += OnTurnStarted;
        _shotEvent.WeaponShot += OnWeaponShot;
        _selectedEvent.WeaponSelected += OnSelected;
    }
    
    private void OnDestroy()
    {
        if(_gameEvents != null) _gameEvents.TurnStarted -= OnTurnStarted;

        if (_shotEvent != null) _shotEvent.WeaponShot -= OnWeaponShot;

        if (_selectedEvent != null) _selectedEvent.WeaponSelected -= OnSelected;
        
        _itemFactory.Dispose();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && _canOpen)
        {
            Toggle();
        }
    }

    private void OnSelected(Weapon weapon)
    {
        Close();
        WeaponSelected?.Invoke(weapon);
    }

    private void Toggle()
    {
        _animator.SetBool("Opened", !_animator.GetBool("Opened"));
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

    private void OnWeaponShot(Weapon arg0)
    {
        _canOpen = false;
    }
}
