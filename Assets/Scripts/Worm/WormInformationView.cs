using TMPro;
using UnityEngine;

public class WormInformationView : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Worm _worm;

    public void Init(Color color, string wormName)
    {
        _healthText.color = color;
        _nameText.color = color;
        _nameText.text = wormName;

        OnHealthChanged(_worm);
    }

    private void OnEnable()
    {
        _worm.DamageTook += OnHealthChanged;
    }

    private void OnDisable()
    {
        _worm.DamageTook -= OnHealthChanged;
    }

    private void OnHealthChanged(Worm worm)
    {
        _healthText.text = worm.Health.ToString();
    }
}
