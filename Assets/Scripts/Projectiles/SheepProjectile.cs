using UnityEngine;

public class SheepProjectile : Projectile
{
    [SerializeField] private SheepMovement _sheepMovement;

    private Timer _timer = new();

    public override void Launch(float currentShotPower, Vector3 spawnPoint, Vector3 direction)
    {
        base.Launch(currentShotPower, spawnPoint, direction);

        _sheepMovement.Reset();
        _sheepMovement.TryMove(Velocity.x / Mathf.Abs(Velocity.x));

        _timer.Start(ProjectileConfig.ExplodeDelay, Explode);
    }

    private void Update()
    {
        _timer.Tick();

        if (Input.GetKeyDown(KeyCode.Space))
            Explode();
    }
}
