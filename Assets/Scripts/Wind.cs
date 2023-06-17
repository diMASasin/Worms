using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wind : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private float _maxVelocity = 2;
    [SerializeField] private float _step = 0.1f;

    public float Velocity { get; set; }

    public float MaxVelocity => _maxVelocity;

    public event Action<float> VelocityChanged;

    private void OnValidate()
    {
        _game = FindObjectOfType<Game>();
    }

    private void OnEnable()
    {
        _game.NextTurnStarted += OnNextTurnStarted;
    }

    private void OnDisable()
    {
        _game.NextTurnStarted -= OnNextTurnStarted;
    }

    private void OnNextTurnStarted()
    {
        Velocity = Random.Range(-_maxVelocity, _maxVelocity);
        Velocity = (int)(Velocity / _step) * _step;
        VelocityChanged?.Invoke(Velocity);
    }
}
