using System;
using System.Collections.Generic;
using EventProviders;
using UI;
using UnityEngine;
using Weapons;
using static UnityEngine.Object;

namespace Factories
{
    public class WeaponSelectorItemFactory : IWeaponSelectedEvent, IDisposable
    {
        private readonly List<WeaponSelectorItem> _items = new();
        private readonly WeaponSelectorItem _itemPrefab;
        private readonly Transform _itemParent;

        public event Action<Weapon> WeaponSelected;

        public WeaponSelectorItemFactory(WeaponSelectorItem itemPrefab, Transform itemParent)
        {
            _itemParent = itemParent;
            _itemPrefab = itemPrefab;
        }
        
        public void Create(IEnumerable<Weapon> weaponList)
        {
            foreach (var weapon in weaponList)
            {
                WeaponSelectorItem weaponItem = Instantiate(_itemPrefab, _itemParent);
                weaponItem.Init(weapon);
                _items.Add(weaponItem);

                weaponItem.Selected += OnSelected;
            }
        }

        public void Dispose()
        {
            

            foreach (var item in _items)
                item.Selected -= OnSelected;
        }

        private void OnSelected(Weapon weapon)
        {
            WeaponSelected?.Invoke(weapon);
        }
    }
}