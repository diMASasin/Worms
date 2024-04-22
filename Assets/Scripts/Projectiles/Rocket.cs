using UnityEngine;

public class Rocket : Projectile
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }
}
