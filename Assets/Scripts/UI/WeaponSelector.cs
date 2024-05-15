using System;
using System.Collections.Generic;
using EventProviders;
using Factories;
using UnityEngine;

public class WeaponSelector : MonoBehaviour, IWeaponSelectedEvent
{
    [SerializeField] Animator _animator;
    [SerializeField] private WeaponSelectorItemFactory _itemFactory;
    [SerializeField] private Transform _itemContainer;
    
    private bool _canOpen = false;
    private IWeaponShotEvent _shotEvent;
    private IGameEventsProvider _gameEvents;
    private IWeaponSelectedEvent _selectedEvent;
    private List<Weapon> _weaponList;
    
    private static readonly int Opened = Animator.StringToHash("Opened");

    public event Action<Weapon> WeaponSelected;

    public void Init(List<Weapon> weaponList)
    {
        _weaponList = weaponList;
        _selectedEvent = _itemFactory;
        
        _itemFactory.Create(weaponList, _itemContainer);

        _selectedEvent.WeaponSelected += OnSelected;
    }
    
    private void OnDestroy()
    {
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
        _animator.SetBool(Opened, !_animator.GetBool(Opened));
    }
    
    private void Close()
    {
        _animator.SetBool(Opened, false);
    }

    public void Enable()
    {
        Close();
        _canOpen = true;
    }

    public void Disable()
    {
        _canOpen = false;
    }
}
