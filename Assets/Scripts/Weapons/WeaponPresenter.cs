using System;
using Pools;
using Projectiles;

namespace Weapons
{
    public class WeaponPresenter : IDisposable
    {
        private readonly Weapon _weapon;
        private readonly WeaponView _weaponView;
        private readonly ProjectileView _projectileView;
        private readonly ProjectilePresenter _projectilePresenter;

        public WeaponPresenter(Weapon weapon, WeaponView weaponView)
        {
            _weapon = weapon;
            _weaponView = weaponView;

            ProjectilePool projectilePool = _weapon.ProjectilePool;
            Projectile projectile = projectilePool.Get();
            _projectileView = _weaponView.ProjectileViewPool.Get(projectile, projectilePool.Config);
            _projectilePresenter = new ProjectilePresenter(projectile, _projectileView, projectilePool.Config);

            _weaponView.Shot += OnShot;
            _weaponView.WeaponChanged += OnWeaponChanged;
        }

        public void Tick()
        {
            _projectilePresenter.Tick();
        }

        public void Dispose()
        {
            _projectilePresenter.Dispose();
        }

        private void OnWeaponChanged(Weapon weapon)
        {
            ProjectilePresenterConfigurator presenterConfigurator = new(_projectilePresenter);
            presenterConfigurator.Configure(_projectilePresenter.Config);
        }

        private void OnShot(Projectile projectile)
        {
            _projectileView.SetPosition(_weaponView.SpawnPoint);
            _projectileView.OnLaunched(projectile.Velocity);
        }
    }
}