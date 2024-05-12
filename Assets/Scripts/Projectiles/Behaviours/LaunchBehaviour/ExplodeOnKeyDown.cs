using System.Collections;
using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class ExplodeOnKeyDown : ILaunchBehaviour
    {
        private readonly Projectile _projectile;
        private IEnumerator _coroutine;

        public ExplodeOnKeyDown(Projectile projectile)
        {
            _projectile = projectile;
            projectile.Exploded += OnExploded;
        }

        public void OnLaunch(Vector2 velocity)
        {
            _coroutine = WaitKeyDown();
            CoroutinePerformer.StartCoroutine(_coroutine);
        }

        private void OnExploded(Projectile projectile)
        {
            projectile.Exploded -= OnExploded;
            CoroutinePerformer.StopCoroutine(_coroutine);
        }

        private IEnumerator WaitKeyDown()
        {
            while (Input.GetKeyDown(KeyCode.Space) == false)
            {
                yield return null;
            }

            _projectile.Explode();
        }
    }
}