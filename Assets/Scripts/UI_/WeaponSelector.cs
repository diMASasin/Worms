using System;
using EventProviders;
using InputService;
using UnityEngine;
using Weapons;
using Zenject;

namespace UI_
{
    public class WeaponSelector : MonoBehaviour, IWeaponSelectorEvents
    {
        [SerializeField] private Animator _animator;
        [field: SerializeField] public Transform ItemParent { get; private set; }

        private IWeaponShotEvent _shotEvent;
        private IWeaponSelectedEvent _selectedEvent;
        private IWeaponSelectorInput _weaponSelectorInput;
        private bool _canOpen;
        private bool _isOpened;

        public event Action SelectorOpened;
        public event Action SelectorClosed;

        private static readonly int Open = Animator.StringToHash("Open");
        private static readonly int Close = Animator.StringToHash("Close");

        [Inject]
        public void Construct(IWeaponSelectorInput weaponSelectorInput, IWeaponSelectedEvent selectedEvent)
        {
            _weaponSelectorInput = weaponSelectorInput;
            _selectedEvent = selectedEvent;
            
            _weaponSelectorInput.ShouldTogleWeaponSelector += Toggle;
            _selectedEvent.WeaponSelected += OnSelected;
        }

        private void OnDestroy()
        {
            if (_selectedEvent != null) _selectedEvent.WeaponSelected -= OnSelected;
            if (_weaponSelectorInput != null) _weaponSelectorInput.ShouldTogleWeaponSelector -= Toggle;
        }

        public void AllowOpen() => _canOpen = true;

        public void DisallowOpen() => _canOpen = false;

        public void Toggle()
        {
            if (_canOpen == false)
            {
                CloseIfOpened();
                return;
            }

            if (_isOpened)
                CloseIfOpened();
            else
                OpenIfClosed();
        }

        public void OpenIfClosed()
        {
            _isOpened = true;
            _animator.ResetTrigger(Close);
            _animator.SetTrigger(Open);
            SelectorOpened?.Invoke();
        }

        public void CloseIfOpened()
        {
            _isOpened = false;
            _animator.ResetTrigger(Open);
            _animator.SetTrigger(Close);
            SelectorClosed?.Invoke();
        }

        private void OnSelected(Weapon weapon) => CloseIfOpened();
    }
}