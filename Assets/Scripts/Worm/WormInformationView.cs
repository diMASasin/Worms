using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WormInformationView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Worm _worm;

    public void Init(Color color)
    {
        _text.color = color;
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
        _text.text = health.ToString();
    }
}
