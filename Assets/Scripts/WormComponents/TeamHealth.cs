using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WormComponents
{
    public class TeamHealth : MonoBehaviour
    {
        [SerializeField] private TMP_Text _teamName;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Slider _healthSlider;

        private int _maxHealth;

        public void Init(Color color, Team team)
        {
            _fillImage.color = color;
            _teamName.text = team.Name;
            _maxHealth = team.MaxHealth;
            team.HealthChanged += OnHealthChanged;
            team.Died += OnDied;
        }

        private void OnHealthChanged(int health)
        {
            _healthSlider.value = (float)health / _maxHealth;
        }

        private void OnDied(Team team)
        {
            team.HealthChanged -= OnHealthChanged;
            team.Died -= OnDied;
        }
    }
}
