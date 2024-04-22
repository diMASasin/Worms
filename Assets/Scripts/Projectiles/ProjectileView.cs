using System;
using Configs;
using DefaultNamespace;
using Pools;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Projectiles
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _text;
        
        private Projectile _projectile;

        public void Init(Projectile projectile)
        {
            _projectile = projectile;
            _spriteRenderer.sprite = _projectile.Config.Sprite;

            _projectile.Exploded += OnExploded;
            _projectile.Timer.TimerUpdated += OnTimerUpdated;
            _projectile.RotationChanged += OnRotationChanged;
        }

        private void OnRotationChanged(Quaternion rotation)
        {
            _spriteRenderer.transform.rotation = rotation;
        }

        private void OnTimerUpdated(float timeLeft)
        {
            _text.text = timeLeft.ToString();
        }

        private void OnExploded(Projectile projectile)
        {
            _canvas.gameObject.SetActive(false);
        }
    }
}