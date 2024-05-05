using Configs;

namespace Projectiles
{
    public class ProjectilePresenterConfigurator
    {
        private readonly ProjectileView _view;
        private readonly Projectile _projectile;
        private ProjectilePresenter _presenter;

        public ProjectilePresenterConfigurator(ProjectilePresenter presenter)
        {
            _view = _presenter.View;
            _projectile = _presenter.Projectile;
            _presenter = presenter;
        }

        public void Configure(ProjectileConfig config)
        {
            if (config.CanWalk == true)
                _presenter.AddLaunchBehaviour(GetMovementBehaviour());

            if (config.HasFragments == true)
                _presenter.AddExplodeBehaviour(GetFragmentsBehaviour());

            if (config.TorqueRange.StartValue != 0 || config.TorqueRange.EndValue != 0)
                _presenter.AddLaunchBehaviour(GetRotator());
        }

        public SheepProjectile GetMovementBehaviour()
        {
            MovementConfig movementConfig = _presenter.Config.MovementConfig;

            var groundChecker = new GroundChecker(_view.transform, _view.Collider, movementConfig.GroundCheckerConfig);
            var sheepMovement = new SheepMovement(_view.Rigidbody, _view.Collider,
                _view.transform, groundChecker, movementConfig);

            SheepProjectile sheepProjectile = new SheepProjectile(sheepMovement);

            return sheepProjectile;
        }

        public ProjectileRotator GetRotator() => new (_presenter.Config, _view.Rigidbody);

        public FragmentsExplodeBehaviour GetFragmentsBehaviour() => new(_presenter.Config.FragmentsPool, _presenter.Config.FragmentsAmount, _view.transform);
    }
}