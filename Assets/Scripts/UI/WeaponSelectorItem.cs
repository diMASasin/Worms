using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectorItem : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;

    private WeaponSelector _weaponSelector;
    private Worm _currentWorm;
    private Weapon _weapon;

    public void Init(WeaponSelector weaponSelector, Weapon weapon)
    {
        _weapon = weapon;
        _weaponSelector = weaponSelector;

        weaponSelector.TurnStarted += OnTurnStarted;

        _image.sprite = _weapon.Config.Sprite;
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnWeaponItemButtonClicked);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnWeaponItemButtonClicked);
    }

    private void OnDestroy()
    {
        if (_weaponSelector != null)
           _weaponSelector.TurnStarted -= OnTurnStarted;
    }

    private void OnTurnStarted(Worm worm)
    {
        _currentWorm = worm;
    }

    private void OnWeaponItemButtonClicked()
    {
        _currentWorm.ChangeWeapon(_weapon);
        _weaponSelector.Toggle();
    }
}
