using TMPro;
using UnityEngine;

namespace WormComponents
{
    public class WormInformationView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _nameText;
    
        private IWorm _worm;

        public void Init(IWorm worm, Color color, string wormName)
        {
            _worm = worm;
            _worm.DamageTook += OnHealthChanged;
        
            _healthText.color = color;
            _nameText.color = color;
            _nameText.text = wormName;
        
            OnHealthChanged(_worm);
        }

        private void OnDestroy() => _worm.DamageTook -= OnHealthChanged;

        private void OnHealthChanged(IWorm worm)
        {
            _healthText.text = worm.Health.ToString();
        }
    }
}
