using UnityEngine;

public class GranadeProjectile : Projectile
{
    [SerializeField] private FollowingObject _canvas;
    [SerializeField] private TimerView _timerView;

    private readonly Timer _timer = new();

    private void Awake()
    {
        _timerView.Init(_timer);
    }

    private void Update()
    {
        _timer.Tick();
    }

    private void OnEnable()
    {
        Exploded += OnExploded;
        _timer.Elapsed += OnElapsed;
    }

    private void OnDisable()
    {
        Exploded -= OnExploded;
        _timer.Elapsed -= OnElapsed;
    }

    public override void OnShot()
    {
        var torque = Random.Range(5, 7);
        Rigidbody2D.AddTorque(Random.Range(-torque, torque));
        _canvas.transform.parent = null;
        _canvas.gameObject.SetActive(true);

        _timer.Start(ProjectileConfig.ExplodeDelay);
    }

    protected virtual void OnElapsed()
    {
        Explode();
    }

    private void OnExploded(Projectile projectile)
    {
        _canvas.gameObject.SetActive(false);
    }
}
