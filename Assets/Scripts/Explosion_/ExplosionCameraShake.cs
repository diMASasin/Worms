using Cinemachine;
using UnityEngine;

namespace Explosion_
{
    public class ExplosionCameraShake : MonoBehaviour
    {
        [SerializeField] private Explosion _explosion;
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private float _cameraShakeFactor = 0.1f;

        private void OnEnable()
        {
            _explosion.Exploded += OnExploded;
        }

        private void OnDisable()
        {
            _explosion.Exploded -= OnExploded;
        }

        private void OnExploded(Explosion explosion) => Shake();

        public void Shake()
        {
            Vector3 impulseVelocity = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
            _impulseSource.GenerateImpulseAt(transform.position, impulseVelocity * _cameraShakeFactor);
        }
    }
}