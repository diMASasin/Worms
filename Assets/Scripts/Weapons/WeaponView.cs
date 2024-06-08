using System;
using Configs;
using EventProviders;
using Pools;
using Projectiles;
using UnityEngine;
using WormComponents;

namespace Weapons
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private Renderer _pointerRenderer;
        [SerializeField] private Transform _pointerLine;
        [SerializeField] private SpriteRenderer _gunSprite;
        [SerializeField] private SpriteRenderer _aimSprite;
        
        private void OnEnable()
        {
            _weapon.ScopeMoved += MoveScope;
            _weapon.Shot += OnShot;
            _weapon.IncreasePowerStarted += OnIncreasePowerStarted;
            _weapon.ShotPowerChanged += OnShotPowerChanged;

        }

        private void OnDisable()
        {
            _weapon.ScopeMoved -= MoveScope;
            _weapon.Shot -= OnShot;
            _weapon.IncreasePowerStarted -= OnIncreasePowerStarted;
            _weapon.ShotPowerChanged -= OnShotPowerChanged;
        }

        private void MoveScope(float zRotation)
        {
            transform.Rotate(0, 0, zRotation * Time.deltaTime);
        }

        private void OnShot(float arg0, Weapon weapon)
        {
            Hide();
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