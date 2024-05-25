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

        public void Init(IWeaponSelectedEvent selectedEvent)
        {
            _selectedEvent = selectedEvent;

            _selectedEvent.WeaponSelected += OnSelected;
        }

        private void OnDestroy()
        {
            if (_selectedEvent != null) _selectedEvent.WeaponSelected -= OnSelected;
        }

        public void Toggle() => _animator.SetBool(Opened, !_animator.GetBool(Opened));

        public void Close() => _animator.SetBool(Opened, false);

        private void OnSelected(Weapon weapon, ProjectilePool projectilePool) => Close();
    }
}
