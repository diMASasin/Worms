using Pools;
using Projectiles;

namespace Weapons
{
    public class WeaponChanger
    {
        private readonly IWeaponSelectedEventProvider _selectEvent;
        private readonly Game _game;
        private readonly ProjectilePool _projectilePool;

        public WeaponChanger(IWeaponSelectedEventProvider selectEvent, Game game, ProjectilePool projectilePool)
        {
            _selectEvent = selectEvent;
            _game = game;
            _projectilePool = projectilePool;


            _selectEvent.WeaponSelected += OnWeaponSelect;
        }

        private void OnWeaponSelect(Weapon weapon)
        {
            Projectile projectile = _projectilePool.Get();
            _game.CurrentWorm.ChangeWeapon(weapon);
            weapon.Reload(projectile);
        }
    }
}