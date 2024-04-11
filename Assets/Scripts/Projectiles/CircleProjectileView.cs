using UnityEngine;

namespace DefaultNamespace.Projectiles
{
    public class CircleProjectileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private CircleCollider2D _collider;

        private Projectile _projectile;

        public void Init(Projectile projectile)
        {
            _projectile = projectile;
            _spriteRenderer.sprite = _projectile.ProjectileConfig.Sprite;
            _collider.radius = _projectile.ProjectileConfig.ColliderRadius;
        }


    }
}