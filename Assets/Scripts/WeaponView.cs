using Configs;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Renderer _pointerRenderer;
    [SerializeField] private Transform _pointerLine;
    [SerializeField] private SpriteRenderer _gunSprite;
    [SerializeField] private SpriteRenderer _aimSprite;

    private Weapon _weapon;

    private void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        TryUnsubscribeWeapon();
    }

    public void OnGunChanged(Weapon weapon)
    {
        TryUnsubscribeWeapon();

        _weapon = weapon;
        _gunSprite.enabled = true;
        _gunSprite.sprite = _weapon.Config.Sprite;

        _weapon.ShotPowerChanged += OnShotPowerChanged;
        _weapon.Shot += OnShot;
        _weapon.PointerLineEnabled += OnPointerLineEnabled;

        _aimSprite.enabled = true;
    }

    public void MoveScope(float direction)
    {
        transform.Rotate(new Vector3(0, 0, -direction * _weapon.Config.ScopeSensetivity) * Time.deltaTime);
    }

    private void OnShot(Projectile projectile)
    {
        projectile.transform.position = _spawnPoint.position;
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
    }
}