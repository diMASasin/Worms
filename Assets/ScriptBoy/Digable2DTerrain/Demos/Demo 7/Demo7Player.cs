using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptBoy.Digable2DTerrain;// You'll need to include this namespace

public class Demo7Player : MonoBehaviour
{
    // This needs to be assigned to in the inspector
    public Shovel shovel;

    public new ParticleSystem particleSystem;

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;


        if (Input.GetMouseButtonDown(0))
        {
            float diggedArea;
            if (shovel.Dig(out diggedArea))
            {
                Debug.Log("diggedArea : " + diggedArea);
                if (diggedArea > 0.05f)
                {
                    //Play ParticleSystem
                    var emission = particleSystem.emission;
                    emission.rateOverTime = 700 * diggedArea;
                    particleSystem.Play();
                }
            }
        }
    }
}