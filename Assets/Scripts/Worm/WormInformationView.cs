using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WormInformationView : MonoBehaviour
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Worm _worm;

    public void Init(Color color, string name)
    {
        _healthText.color = color;
        _nameText.color = color;

        _nameText.text = name;
    }

    private void OnEnable()
    {
        _worm.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        _worm.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int health)
    {
        _healthText.text = health.ToString();
    }
}
