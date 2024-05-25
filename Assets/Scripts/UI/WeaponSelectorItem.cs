using System;
using System.Collections.Generic;
using Configs;
using Pools;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI
{
    public class WeaponSelectorItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private Weapon _weapon;
        private ProjectilePool _projectilePool;

        public event Action<Weapon, ProjectilePool> Selected;

        public void Init(Weapon weapon, ProjectilePool projectilePool)
        {
            _weapon = weapon;
            _projectilePool = projectilePool;

            _image.sprite = _weapon.Config.Sprite;
            _image.gameObject.transform.localScale *= _weapon.Config.SpriteScale;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnWeaponItemButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnWeaponItemButtonClicked);
        }

        private void OnWeaponItemButtonClicked()
        {
            Selected?.Invoke(_weapon, _projectilePool);
        }
    }
}
