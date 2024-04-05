using ScriptBoy.Digable2DTerrain;
using UnityEngine;

public class FragmentationGranadeProjectile : GranadeProjectile
{
  protected override void OnElapsed()
    {
        base.OnElapsed();
        for (int i = 0; i < ProjectileConfig.FragmentsAmount; i++)
        {
            var fragment = FragmentsPool.Pool.Get();
            fragment.transform.position = transform.position;
            fragment.gameObject.SetActive(true);
            fragment.Rigidbody2D.velocity += new Vector2(Random.Range(-2f, 2f), Random.Range(4f, 6f));
            fragment.Exploded += Remove;
        }
    }

    private void Remove(Projectile projectile)
    {
        projectile.Exploded -= Remove;
        FragmentsPool.OnRemoved(projectile);
    }
}
