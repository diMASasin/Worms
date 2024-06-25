using TMPro;
using UnityEngine;

namespace WormComponents
{
    public class WormInformationView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private FollowingObject _followingObject;
    
        private Worm _worm;

        public void Init(Worm worm, Color color, string wormName)
        {
            _worm = worm;
            _worm.DamageTook += OnHealthChanged;
        
            _healthText.color = color;
            _nameText.color = color;
            _nameText.text = wormName;

            _followingObject.Follow(_worm.Transform);
            
            OnHealthChanged(_worm);
        }

        private void OnDestroy() => _worm.DamageTook -= OnHealthChanged;
        
        private void OnHealthChanged(Worm worm)
        {
            _healthText.text = worm.Health.ToString();
        }
    }
}
