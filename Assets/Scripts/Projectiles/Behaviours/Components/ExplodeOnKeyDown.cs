using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Projectiles.Behaviours.Components
{
    public class ExplodeOnKeyDown : MonoBehaviour
    {
        [SerializeField] private Projectile _projectile;
        
        private Coroutine _coroutine;
        private readonly float _secondsToEnableKey = 1;

        private void OnEnable()
        {
            _projectile.Launched += OnLaunch;
        }

        private void OnDisable()
        {
            _projectile.Launched -= OnLaunch;
        }

        private void OnLaunch(Projectile projectile, Vector2 vector2)
        {
            WaitKeyDown().Forget();
        }

        private async UniTaskVoid WaitKeyDown()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_secondsToEnableKey));

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) == true);

            _projectile.Explode();
        }
    }
}