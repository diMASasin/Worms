using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : Projectile
{
    private void FixedUpdate()
    {
        SpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.forward, Rigidbody2D.velocity);
    }
}
