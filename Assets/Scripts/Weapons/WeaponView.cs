using System;
using Pools;
using Projectiles;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Renderer _pointerRenderer;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private SpriteRenderer _gunSprite;
    [SerializeField] private SpriteRenderer _aimSprite;
    [SerializeField] private ProjectilePool _pool;

    public Transform SpawnPoint => _spawnPoint;
    public ProjectilePool ProjectileViewPool => _pool;

    public Action<Projectile> Shot;

    private void Start()
    {
        Hide();
    }

    public void EnableAimSprite()
    {
        _aimSprite.enabled = true;
    }

    public void SetGunSprite(Sprite sprite)
    {
        _gunSprite.enabled = true;
        _gunSprite.sprite = sprite;
    }

    public void MoveScope(float zRotation)
    {
        transform.Rotate(0, 0, zRotation * Time.deltaTime);
    }

    public void OnShot(Projectile projectile)
    {
        Hide();

        Shot?.Invoke(projectile);
    }

    public void OnShotPowerChanged(float normalizedShotPower)
    {
        _pointerLine.localScale = new Vector3(normalizedShotPower, 1, 1);
    }

    private void Hide()
    {
        _pointerRenderer.enabled = false;
        _aimSprite.enabled = false;
        _gunSprite.enabled = false;
    }

    public void OnPointerLineEnabled()
    {
        _pointerRenderer.enabled = true;
    }
}