using ScriptBoy.Digable2DTerrain.Scripts;
using UnityEngine;

// You'll need to include this namespace

namespace ScriptBoy.Digable2DTerrain.Demos.Demo_4
{
    public class Demo4Player : MonoBehaviour
    {
        // This needs to be assigned to in the inspector
        public Shovel shovel;

        //Dig ground by pressing this key
        public KeyCode DigKey;

        //Move speed of the character
        public float moveSpeed;

        private Rigidbody2D m_Rigidbody2D;

        void Start()
        {
            //Get the Rigidbody2D attached to the GameObject
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            //Detect when the DigKey is pressed down
            if (Input.GetKeyDown(DigKey))
            {
                //Dig all terrains inside the shovel circle
                shovel.Dig();
            }

            //Move the character based on the horizontal axis
            float move = Input.GetAxis("Horizontal") * moveSpeed;
            m_Rigidbody2D.velocity = new Vector2(move , m_Rigidbody2D.velocity.y);
        }
    }
}