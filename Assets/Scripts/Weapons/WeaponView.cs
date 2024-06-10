using UnityEngine;

namespace Weapons
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        [SerializeField] private ShotPowerIncreaser _shotPowerIncreaser;
        [SerializeField] private Transform _pointerLine;
        
        private void OnEnable()
        {
            _shotPowerIncreaser.ShotPowerChanged += OnShotPowerChanged;
        }

        private void OnDisable()
        {
            _shotPowerIncreaser.ShotPowerChanged -= OnShotPowerChanged;
        }

        private void OnShotPowerChanged(float currentShotPower)
        {
            _pointerLine.localScale = new Vector3(currentShotPower / _weapon.Config.MaxShotPower, 1, 1);
        }
    }
}