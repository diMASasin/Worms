using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;

// You'll need to include this namespace

//Note : In this demo we don't call the shovel.Dig() directly in the script
//The Dig function will be called by an Animation Event

namespace ScriptBoy.Digable2DTerrain.Demos.Demo_5
{
    public class Demo5Player : MonoBehaviour
    {
        // This needs to be assigned to in the inspector
        public Shovel shovel;

        //Start Dig animation by pressing this key
        public KeyCode DigKey;

        public ParticleSystem dirtParticleSystem;
        private CharacterController2D characterController;
        private Animator animator;

        //Move speed of the character
        public float moveSpeed;

        //This function called by an Animation Event in "Demo 5/Animations/Dig.anim" animation clip
        private void Dig()
        {
            shovel.Dig();
        }

        //This function called by an Animation Event in "Demo 5/Animations/Dig.anim" animation clip
        private void StartParticleSystem()
        {
            //Start dirt Particle System while digging ground
            dirtParticleSystem.Play();
        }

        //This function called by an Animation Event in "Demo 5/Animations/Dig.anim" animation clip
        private void StopParticleSystem()
        {
            //Stop dirt Particle System
            dirtParticleSystem.Stop();
        }

        private void Start()
        {
            //Get the Animator attached to the GameObject
            animator = GetComponent<Animator>();
            //Get the CharacterController2D attached to the GameObject
            characterController = GetComponent<CharacterController2D>();
        }

        private void Update () 
        {
            if (characterController.Grounded)
            {
                // We are grounded

                //Detect when the DigKey is pressed down
                if (Input.GetKeyDown(DigKey))
                {
                    //Send the message to the Animator to activate the trigger parameter named "Dig".
                    //Change the settings in the Animator to play "Dig" animation clip.
                    animator.SetTrigger("Dig");
                }

                //Translate the left and right button presses or the horizontal joystick movements to a float
                float horizontalAxis = Input.GetAxis("Horizontal");
                //Move the character based on the horizontal axis
                characterController.Move(horizontalAxis * moveSpeed);
                //Sends the value from the horizontal axis input to the animator. Change the settings in the
                //Animator to define when the character is running
                animator.SetFloat("Run", Mathf.Abs(horizontalAxis));
            }
            else
            {
                //We are falling, so stop running animation
                animator.SetFloat("Run", 0);
            }
        }
    }
}