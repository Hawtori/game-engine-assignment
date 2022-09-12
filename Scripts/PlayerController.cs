using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
 
    /***********************************************************************************
                                THINGS TO DO
    - fix jumping to feel better
    - fix countermovement and friction stuff so movement feels better, 
          slows and stops faster but not choppy
    - improve overall game feel
    
                                MABIES
    - add a slow effect for when landing (unless bunny hopping so falling makes player slow on impact)
    - add crouching

    ***********************************************************************************/

    //assign from unity
    public Transform playerCam;
    public Transform orientation;

    public Rigidbody rb;

    //look around
    public float sensX = 50f, sensY = 50f; //so we can edit horizontal and vertical sens independently

    private float xRotation, desiredY;
    private float sensMultiplier = 1f;

    //move around
    public float moveSpeed = 2; //put high number so acceleration isn't slow
    public float maxSpeed = 20;
    public bool grounded;
    //public LayerMask _isGround; //this can be used later on depending on what direction the game goes
    private float speedMultiplier = 1; //1 for main weapon, 1.5 for pistol, 2.125 for knife or something like that

    //jump
    [SerializeField]
    private float jumpForce = 1000f;
    private static float jumpCD = 0.25f; //set time
    private float jumpCdTimer = 0.25f; //timer for when to reset
    private bool canJump = true;

    //inputs
    private float x, y;
    private bool jump;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //hide and lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //transform.localRotation = orientation.localRotation;
        if(canJump == false)
        {
            if(jumpCdTimer < 0f)
            {
                canJump = true;
                jumpCdTimer = jumpCD;
            }
            else
            {
                jumpCdTimer -= Time.deltaTime;
            }
        }

        GetInputs();
        Look();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) grounded = false;
    }

    /// <summary>
    /// Get user inputs for movement
    /// </summary>
    private void GetInputs()
    {
        x = Input.GetAxis("Horizontal"); //not axisRaw so we can potentially use controller too
        y = Input.GetAxis("Vertical");
        jump = Input.GetButton("Jump");

        //normalize movement so diagnal movement isn't faster
        Vector2 normalizedMovement = new Vector2(x, y);
        normalizedMovement.Normalize();

        x = normalizedMovement.x; y = normalizedMovement.y;
    }

    /// <summary>
    /// Get mouse inputs to look around
    /// </summary>
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.fixedDeltaTime * sensMultiplier;

        Vector3 rotation = playerCam.transform.localRotation.eulerAngles;
        desiredY = rotation.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //lock rotation

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredY, 0);
        //orientation.transform.localRotation = Quaternion.Euler(0, desiredY, 0);
        transform.localRotation = Quaternion.Euler(0, desiredY, 0);
    }

    /// <summary>
    /// Execute the movement
    /// </summary>
    private void Move()
    {

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        if (canJump && jump) Jump();

        //slow down if key released
        //if (grounded && x == 0 && y == 0 && Mathf.Abs(rb.velocity.magnitude) > 0) { rb.velocity = rb.velocity / divFactor; divFactor = Mathf.Clamp(divFactor - 0.5f, 1f, 5f); }
        if (Mathf.Abs(xMag) > 0.2 && Mathf.Abs(x) < 0.05f || (xMag < -0.2 && x > 0) || (xMag > 0.2 && x < 0)) rb.AddForce(moveSpeed * orientation.transform.right * -xMag * 0.2f);
        if (Mathf.Abs(yMag) > 0.2 && Mathf.Abs(y) < 0.05f || (yMag < -0.2 && y > 0) || (yMag > 0.2 && y < 0)) rb.AddForce(moveSpeed * orientation.transform.forward * -yMag * 0.2f);

        //don't take input if over max speed;
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        rb.AddForce(orientation.transform.forward * y * moveSpeed /* Time.deltaTime */ * speedMultiplier);
        rb.AddForce(orientation.transform.right * x * moveSpeed /** Time.deltaTime*/ * speedMultiplier);
    }

    private void Jump()
    {
        if(grounded && canJump)
        {
            canJump = false;

            rb.AddForce(Vector2.up * jumpForce * 2f);

            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
        }
    }

    /// <summary>
    /// Find velocity relative to where we're looking.
    /// Good for vector calculations and stuff
    /// </summary>
    private Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
}
