using System.Collections;
using Infrastructure;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Projectiles.Behaviours.Components
{
    public class ExplodeOnKeyDown : MonoBehaviour
    {
        [SerializeField] private Projectile _projectile;
        private Coroutine _coroutine;

        public void Init(Projectile projectile)
        {
            _projectile = projectile;
        }

        private void OnEnable()
        {
            _projectile.Launched += OnLaunch;
            _projectile.Exploded += OnExploded;
        }

        private void OnDisable()
        {
            _projectile.Launched -= OnLaunch;
            _projectile.Exploded -= OnExploded;
        }

        public void OnLaunch(Projectile projectile, Vector2 vector2)
        {
            _coroutine = CoroutinePerformer.StartCoroutine(WaitKeyDown());
        }

        private void OnExploded(Projectile projectile)
        {
            CoroutinePerformer.StopCoroutine(_coroutine);
        }

        private IEnumerator WaitKeyDown()
        {
            while (Input.GetKeyDown(KeyCode.Space) == false)
                yield return null;

            _projectile.Explode();
        }
    }
}