using System;
using System.Collections.Generic;
using Configs;
using EventProviders;
using Pools;
using UI;
using UnityEngine;
using Weapons;
using static UnityEngine.Object;

namespace Factories
{
    public class WeaponSelectorItemFactory : IWeaponSelectedEvent, IDisposable
    {
        private readonly List<WeaponSelectorItem> _items = new();

        public event Action<Weapon> WeaponSelected;

        public void Create(IEnumerable<Weapon> weaponList, WeaponSelectorItem itemPrefab, Transform itemParent)
        {
            foreach (var weapon in weaponList)
            {
                WeaponSelectorItem weaponItem = Instantiate(itemPrefab, itemParent);
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