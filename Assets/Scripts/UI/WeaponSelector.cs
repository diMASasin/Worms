using Configs;
using EventProviders;
using Pools;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace UI
{
    public class WeaponSelector : MonoBehaviour, IWeaponSelectorOpener
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Image _image;
        [SerializeField] private UIConfig _config;
        
        [field: SerializeField] public Transform ItemParent { get; private set; }
        
        private IWeaponShotEvent _shotEvent;
        private IWeaponSelectedEvent _selectedEvent;
    
        private static readonly int Opened = Animator.StringToHash("Opened");
        private IWeaponSelectorInput _weaponSelectorInput;

        public void Init(IWeaponSelectedEvent selectedEvent, IWeaponSelectorInput weaponSelectorInput)
        {
            _weaponSelectorInput = weaponSelectorInput;
            _selectedEvent = selectedEvent;

            _selectedEvent.WeaponSelected += OnSelected;
            _weaponSelectorInput.ShouldTogleWeaponSelector += Toggle;
        }

        private void OnDestroy()
        {
            if (_selectedEvent != null) _selectedEvent.WeaponSelected -= OnSelected;
            if (_weaponSelectorInput != null) _weaponSelectorInput.ShouldTogleWeaponSelector -= Toggle;
        }

        public void Toggle() => _animator.SetBool(Opened, !_animator.GetBool(Opened));

        public void Close() => _animator.SetBool(Opened, false);

        private void OnSelected(Weapon weapon) => Close();
    }
}
