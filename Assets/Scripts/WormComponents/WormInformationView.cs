using FollowingObject_;
using R3;
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
        
            _healthText.color = color;
            _nameText.color = color;
            _nameText.text = wormName;

            _worm.Health.Subscribe(OnHealthChanged);
            _followingObject.Follow(_worm.Transform);
        }
        
        private void OnHealthChanged(int health)
        {
            _healthText.text = health.ToString();
        }
    }
}
