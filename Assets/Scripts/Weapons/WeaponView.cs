using System;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using UnityEngine;

namespace Weapons
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Renderer _pointerRenderer;
        [SerializeField] private Transform _pointerLine;
        [SerializeField] private SpriteRenderer _gunSprite;
        [SerializeField] private SpriteRenderer _aimSprite;

        private IWeaponEventsAndConfig _weapon;
        private IWeaponSelectedEvent _weaponSelectedEvent;

        public void Init(IWeaponSelectedEvent weaponSelectedEvent)
        {
            _weaponSelectedEvent = weaponSelectedEvent;
            
            _weaponSelectedEvent.WeaponSelected += OnWeaponChanged;

            Hide();
        }

        private void OnDestroy()
        {
            if (_weaponSelectedEvent != null)
                _weaponSelectedEvent.WeaponSelected -= OnWeaponChanged;
        }

        private void EnableAimSprite()
        {
            _aimSprite.enabled = true;
        }

        private void SetGunSprite(WeaponConfig config)
        {
            _gunSprite.enabled = true;
            _gunSprite.sprite = config.Sprite;
        }

        private void MoveScope(float zRotation)
        {
            transform.Rotate(0, 0, zRotation * Time.deltaTime);
        }

        private void OnShot(float arg0, Weapon weapon)
        {
            Hide();
            TryUnsubscribeWeapon();
        }

        private void OnWeaponChanged(Weapon weapon)
        {
            TryUnsubscribeWeapon();

            _weapon = weapon;

            _weapon.ShotPowerChanged += OnShotPowerChanged;
            _weapon.Shot += OnShot;
            _weapon.IncreasePowerStarted += OnIncreasePowerStarted;
            _weapon.ScopeMoved += MoveScope;

            SetGunSprite(_weapon.Config);
            EnableAimSprite();
        }

        private void TryUnsubscribeWeapon()
        {
            if (_weapon == null)
                return;

            _weapon.ShotPowerChanged -= OnShotPowerChanged;
            _weapon.Shot -= OnShot;
            _weapon.IncreasePowerStarted -= OnIncreasePowerStarted;
            _weapon.ScopeMoved -= MoveScope;
        }

        private void OnShotPowerChanged(float currentShotPower)
        {
            _pointerLine.localScale = new Vector3(currentShotPower / _weapon.Config.MaxShotPower, 1, 1);
        }

        private void Hide()
        {
            _pointerRenderer.enabled = false;
            _aimSprite.enabled = false;
            _gunSprite.enabled = false;
        }

        private void OnIncreasePowerStarted()
        {
            _pointerRenderer.enabled = true;
        }
    }
}