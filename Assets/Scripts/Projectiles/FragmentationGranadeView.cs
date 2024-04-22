using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Projectiles
{
    public class FragmentationGranadeView
    {
        [SerializeField] private ProjectileView _projectileViewPrefab;

        private FragmentsExplodeModifier _fragmentsExplodeModifier;
        private ObjectPool<ProjectileView> _projectileViewPool;
        private Transform _projectileViewPosition;

        private List<Projectile> _fragments = new();

        public void Init(FragmentsExplodeModifier fragmentsExplodeModifier, Transform projectileViewPosition)
        {
            _fragmentsExplodeModifier = fragmentsExplodeModifier;
            _projectileViewPosition = projectileViewPosition;

            _projectileViewPool = new ObjectPool<ProjectileView>(CreateProjectileView);

            _fragmentsExplodeModifier.FragmentsRecieved += ReleaseFragments;
        }

        private void OnExploded(ProjectileView projectileView)
        {
            //projectileView.Exploded -= OnExploded;

            _projectileViewPool.Release(projectileView);
        }

        private ProjectileView CreateProjectileView()
        {
            ProjectileView projectileView = Object.Instantiate(_projectileViewPrefab, _projectileViewPosition.position, Quaternion.identity);
            Projectile fragment = _fragments.First();
            //projectileView.Init(fragment, _projectileViewPosition.position);
            
            //projectileView.Exploded += OnExploded;

            _fragments.Remove(fragment);

            return projectileView;
        }

        private void ReleaseFragments(List<Projectile> fragments)
        {
            _fragmentsExplodeModifier.FragmentsRecieved -= ReleaseFragments;

            _fragments = fragments;

            foreach (var fragment in fragments)
            {
                ProjectileView fragmentView = _projectileViewPool.Get();
                fragmentView.transform.position = _projectileViewPosition.position;
                fragmentView.gameObject.SetActive(true);
            }
        }
    }
}