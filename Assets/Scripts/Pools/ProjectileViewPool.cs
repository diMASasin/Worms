using System;
using Configs;
using Projectiles;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Pools
{
    [CreateAssetMenu(fileName = "ProjectileViewPool", menuName = "ProjectileViewPool", order = 0)]
    public class ProjectileViewPool : ScriptableObject
    {
        [SerializeField] private ProjectileView _prefab;
        [SerializeField] private int _amount;

        private Transform _projectilesParent;
        private ObjectPool<ProjectileView> _pool;
        private ExplosionPool _explosionPool;

        public event Action<ProjectileView> Got;
        public event Action<ProjectileView> Released;

        public void Init(Transform projectilesParent, ExplosionPool explosionPool)
        {
            _projectilesParent = projectilesParent;
            _explosionPool = explosionPool;

            _pool = new ObjectPool<ProjectileView>(OnCreated, OnGet, OnReleased, OnDispose, true, _amount);
        }

        private void OnDestroy()
        {
            _pool.Dispose();
        }

        public ProjectileView Get(Projectile projectile, ProjectileConfig config)
        {
            ProjectileView view = _pool.Get();
            view.Init(config, _explosionPool);

            Got?.Invoke(view);
            return view;
        }

        internal void OnGet(ProjectileView projectileView)
        {
            projectileView.gameObject.SetActive(true);
            projectileView.Exploded += OnExploded;
        }

        public void OnReleased(ProjectileView projectileView)
        {
            projectileView.gameObject.SetActive(false);
            projectileView.ResetView();
            Released?.Invoke(projectileView);
            projectileView.Exploded -= OnExploded;
        }

        private ProjectileView OnCreated()
        {
            ProjectileView view = Object.Instantiate(_prefab, _projectilesParent);


            return view;
        }

        private void OnExploded(ProjectileView projectileView)
        {
            _pool.Release(projectileView);
        }

        private void OnDispose(ProjectileView view)
        {
        }
    }
}