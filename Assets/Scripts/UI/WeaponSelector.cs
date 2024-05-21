using System;
using System.Collections.Generic;
using EventProviders;
using Factories;
using UnityEngine;
using Weapons;

namespace UI
{
    public class WeaponSelector : MonoBehaviour, IWeaponSelectedEvent
    {
        [SerializeField] Animator _animator;
        [SerializeField] private WeaponSelectorItemFactory _itemFactory;
        [SerializeField] private Transform _itemContainer;
    
        private IWeaponShotEvent _shotEvent;
        private IGameEventsProvider _gameEvents;
        private IWeaponSelectedEvent _selectedEvent;
    
        private static readonly int Opened = Animator.StringToHash("Opened");

        public event Action<Weapon> WeaponSelected;

        public void Init(List<Weapon> weaponList)
        {
            _selectedEvent = _itemFactory;
        
            _itemFactory.Create(weaponList, _itemContainer);

            _selectedEvent.WeaponSelected += OnSelected;
        }
    
        private void OnDestroy()
        {
            if (_selectedEvent != null) _selectedEvent.WeaponSelected -= OnSelected;
        
            _itemFactory.Dispose();
        }

        private void OnSelected(Weapon weapon)
        {
            Close();
            WeaponSelected?.Invoke(weapon);
        }

        public void Toggle()
        {
            _animator.SetBool(Opened, !_animator.GetBool(Opened));
        }
    
        public void Close()
        {
            _animator.SetBool(Opened, false);
        }
    }
}
