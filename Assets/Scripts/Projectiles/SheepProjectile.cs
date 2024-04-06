using UnityEngine;

public class SheepProjectile : Projectile
{
    [SerializeField] private SheepMovement _sheepMovement;

    private readonly Timer _timer = new();
    
    private void OnEnable()
    {
        _timer.Elapsed += OnTimerElapsed;
    }

    private void OnDisable()
    {
        _timer.Elapsed -= OnTimerElapsed;
    }

    public override void OnShot()
    {
        _sheepMovement.Reset();
        _sheepMovement.TryMove(Velocity.x / Mathf.Abs(Velocity.x));

        _timer.Start(ProjectileConfig.ExplodeDelay);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Explode();
    }

    private void OnTimerElapsed()
    {
        Explode();
    }
}
