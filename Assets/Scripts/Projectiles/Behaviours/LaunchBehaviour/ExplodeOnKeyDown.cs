using System.Collections;
using Infrastructure;
using UnityEngine;

namespace Projectiles.Behaviours.LaunchBehaviour
{
    public class ExplodeOnKeyDown : ILaunchBehaviour
    {
        private readonly Projectile _projectile;
        private Coroutine _coroutine;

        public ExplodeOnKeyDown(Projectile projectile)
        {
            _projectile = projectile;
            projectile.Exploded += OnExploded;
        }

        public void OnLaunch(Vector2 velocity)
        {
            _coroutine = CoroutinePerformer.StartCoroutine(WaitKeyDown());
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