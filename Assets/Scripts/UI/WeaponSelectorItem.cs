using System;
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

        public event Action<Weapon> Selected;

        public void Init(Weapon weapon)
        {
            _weapon = weapon;

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
            Selected?.Invoke(_weapon);
        }
    }
}
