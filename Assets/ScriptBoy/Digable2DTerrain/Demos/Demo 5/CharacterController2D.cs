using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    // How much to smooth out the movement
    public float MovementSmoothing = 0.1f;
    // Whether or not the player is grounded.
    public bool Grounded;
    // For determining which way the player is currently facing.
    private bool FacingRight = true;

    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity;

    private void Start()
    {
        //Get the Rigidbody2D attached to the GameObject
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Move(float move)
    {
        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
        // And then smoothing it out and applying it to the character
        m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, MovementSmoothing);


        // If the input is moving the player right and the player is facing left...
        if (move > 0 && !FacingRight)
        {
            // ... flip the player.
            Flip();
        }
        // Otherwise if the input is moving the player left and the player is facing right...
        else if (move < 0 && FacingRight)
        {
            // ... flip the player.
            Flip();
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        FacingRight = !FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    // Detect collision with ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            Grounded = true;
        }
    }

    // Detect collision exit with ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            Grounded = false;
        }
    }
}
