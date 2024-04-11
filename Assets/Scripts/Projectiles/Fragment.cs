using UnityEngine;

public class Fragment : Projectile
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }
}
