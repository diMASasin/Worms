using System;
using Pools;
using Projectiles;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Renderer _pointerRenderer;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private SpriteRenderer _gunSprite;
    [SerializeField] private SpriteRenderer _aimSprite;
    [SerializeField] private ProjectileViewPool _pool;

    private Weapon _weapon;

    public Transform SpawnPoint => _spawnPoint;
    public ProjectileViewPool ProjectileViewPool => _pool;

    public Action<Projectile> Shot;
    public Action<Weapon> WeaponChanged;

    private void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        TryUnsubscribeWeapon();
    }

    public void OnWeaponChanged(Weapon weapon)
    {
        TryUnsubscribeWeapon();

        _weapon = weapon;
        _gunSprite.enabled = true;
        _gunSprite.sprite = _weapon.Config.Sprite;

        _weapon.ShotPowerChanged += OnShotPowerChanged;
        _weapon.Shot += OnShot;
        _weapon.PointerLineEnabled += OnPointerLineEnabled;
        _weapon.ScopeMoved += MoveScope;

        _aimSprite.enabled = true;

        WeaponChanged?.Invoke(_weapon);
    }

    private void MoveScope(float zRotation)
    {
        transform.Rotate(0, 0, zRotation * Time.deltaTime);
    }

    private void OnShot(Projectile projectile)
    {
        Hide();
        TryUnsubscribeWeapon();

        Shot?.Invoke(projectile);
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

    private void OnPointerLineEnabled()
    {
        _pointerRenderer.enabled = true;
    }

    private void TryUnsubscribeWeapon()
    {
        if (_weapon == null)
            return;
        
        _weapon.ShotPowerChanged -= OnShotPowerChanged;
        _weapon.Shot -= OnShot;
        _weapon.PointerLineEnabled -= OnPointerLineEnabled;
        _weapon.ScopeMoved -= MoveScope;
    }
}