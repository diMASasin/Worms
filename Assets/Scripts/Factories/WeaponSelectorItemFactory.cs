using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Factories
{
    [CreateAssetMenu(fileName = "WeaponSelectorItemFactory", menuName = "Factories/WeaponSelectorItem")]
    public class WeaponSelectorItemFactory : ScriptableObject, IWeaponSelectedEvent
    {
        [SerializeField] private WeaponSelectorItem _itemPrefab;
        
        private readonly List<WeaponSelectorItem> _items = new();

        public event Action<Weapon> WeaponSelected;
        
        public void Create(List<Weapon> weaponList, Transform itemParent)
        {
            foreach (var weapon in weaponList)
            {
                WeaponSelectorItem weaponItem = Object.Instantiate(_itemPrefab, itemParent);
                weaponItem.Init(weapon);
                _items.Add(weaponItem);
                
                weaponItem.Selected += OnSelected;
            }
        }
        
        public void Dispose()
        {
            if(_items != null)
                foreach (var item in _items)
                    item.Selected -= OnSelected;
        }

        private void OnSelected(Weapon weapon)
        {
            WeaponSelected?.Invoke(weapon);
        }
    }
}