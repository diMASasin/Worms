using UnityEngine;

public class Fragment : Projectile
{
    public override void OnShot()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }
}
