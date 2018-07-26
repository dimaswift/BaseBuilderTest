using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    [Tooltip("Jump force")]
    public float jumpForce = 15;

    [Tooltip("Movement speed")]
    public float speed = 5;
    /// <summary>
    /// Gets or sets current normalized movement direction of the rigidbody. 
    /// </summary>
    public Vector3 MovementDirection { get; set; }

    public new Transform transform { get; private set; }

    Rigidbody body;
    Vector3 previousVelocity;
    bool isGrounded;
    GameObject ground;

    // Caching of all the properties happens here
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        transform = base.transform;
        previousVelocity = MovementDirection;
    }

    public void Jump()
    {
        if(isGrounded)
            body.velocity += Vector3.up * jumpForce;
    }

    private void FixedUpdate()
    {
        ProcessMovement();
    }

    /// <summary>
    /// Moves the player according to its current transform direction, using the input direction and velocity.
    /// </summary>
    void ProcessMovement()
    {
        var newVel = transform.TransformDirection(MovementDirection.normalized) * speed; 
        newVel.y = body.velocity.y;
        body.velocity = newVel;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        // checks if collision is vertical, i.e it's probably is the ground, and store ground object
        var n = collision.contacts[0].normal; 
        if(Vector3.Dot(Vector3.up, n) > .9f)
        {
            isGrounded = true;
            ground = collision.gameObject;
        }
     
    }

    private void OnCollisionExit(Collision collision)
    {
        // only set grounded to false if we lift of from the ground, and not other object
        if (ground == collision.gameObject)
        {
            isGrounded = false;
        }
    }
}
