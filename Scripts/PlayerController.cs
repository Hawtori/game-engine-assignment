using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // control velocity directly this time //
    public Transform eyes;

    public Rigidbody rb;

    //look
    public float xSens = 20f, ySens = 20f; //let player adjust

    private float xRotation, desiredY;
    private float sensMultiplier = 1f; //our sens

    //move
    public float moveSpeed = 1f;
    public float speedMultiplier = 1f; // so we can adjust speed for scenarios (pistol, knife, gun, etc)

    //jump
    public float jumpMultiplier = 2f;

    [SerializeField]
    private float jumpForce = 800f;
    private float jumpCd = 0.25f, jumpTimer;
    private bool canJump = true;
    private bool isGrounded;

    //inputs
    private Vector2 movementInput;
    private bool jump;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!canJump)
        { 
            if (isGrounded && jumpTimer < 0f)
            {
                canJump = true;
                jumpTimer = jumpCd;
            }
            else
                jumpTimer -= Time.deltaTime;  
        }

        //Inputs
        GetInputs();
        Look();
    }


    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Takes in user input for movement
    /// </summary>
    private void GetInputs()
    {
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.y = Input.GetAxis("Vertical");
        jump = Input.GetButton("Jump");

        movementInput.Normalize();
    }

    /// <summary>
    /// Takes in user mouse input to look around, shoot and moves camera
    /// </summary>
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * xSens * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * ySens * Time.fixedDeltaTime * sensMultiplier;

        Vector3 rotation = eyes.transform.localRotation.eulerAngles;
        desiredY = rotation.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //lock rotation

        eyes.transform.localRotation = Quaternion.Euler(xRotation, desiredY, 0);
        transform.localRotation = Quaternion.Euler(0, desiredY, 0);
    }

    /// <summary>
    /// Moves player based on input taken
    /// </summary>
    private void Move()
    {
        rb.velocity = Vector3.zero;

        if (canJump && jump) Jump();

        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y);

        if (Mathf.Abs(rb.velocity.x) > 0.2 && Mathf.Abs(movementInput.x) < 0.05f || (rb.velocity.x < -0.2 && movementInput.x > 0) || (rb.velocity.x > 0.2 && movementInput.x < 0)) rb.AddForce(moveSpeed * transform.right * -rb.velocity.x * 0.2f);
        if (Mathf.Abs(rb.velocity.y) > 0.2 && Mathf.Abs(movementInput.y) < 0.05f || (rb.velocity.y < -0.2 && movementInput.y > 0) || (rb.velocity.y > 0.2 && movementInput.y < 0)) rb.AddForce(moveSpeed * transform.forward * -rb.velocity.y * 0.2f);


        if(rb.velocity.sqrMagnitude > moveSpeed * speedMultiplier)
        {
            rb.velocity = rb.velocity.normalized * moveSpeed * speedMultiplier;
        }
        Vector3 vel = new Vector3( movementInput.x * moveSpeed * speedMultiplier, rb.velocity.y, movementInput.y * moveSpeed * speedMultiplier);

        rb.velocity = vel;
    }

    /// <summary>
    /// Makes player jump if they are allowed to
    /// </summary>
    private void Jump()
    {
        if (!canJump) return;

        canJump = false;
        rb.AddForce(Vector2.up * jumpForce * jumpMultiplier);

        //preserve velocity 
        Vector3 vel = rb.velocity;
        if (rb.velocity.y < 0.5f)
            rb.velocity = new Vector3(vel.x, 0, vel.z);
        else if (rb.velocity.y > 0)
            rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }

}
 
