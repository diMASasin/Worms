using UnityEngine;

public class Rocket : Projectile
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        SpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, Rigidbody2D.velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }
}
