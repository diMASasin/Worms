﻿using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;

// You'll need to include this namespace

namespace ScriptBoy.Digable2DTerrain.Demos.Demo_7
{
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
}