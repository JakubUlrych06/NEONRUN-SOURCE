using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 


public class PlayerMovementAdvanced : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float grappleSpeedMultiplier;
    public float swingSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    private boss boss;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    

    public Transform orientation;
    public Entity entityScript; 


    float horizontalInput;
    float verticalInput;
    public float killsLeft;

    Vector3 moveDirection;  
    [Header("Lost")]

    public Vector3 playerPos;   
    public Transform playerobj;

    Rigidbody rb;
    [Header("States")]

    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        grappling,
        freeze,
        swinging,
        wallrunning,
        crouching,
        sliding,
        air
    }

    public bool startBoss = false; 
    public bool sliding;
    public bool wallrunning;
    public bool swinging;
    public bool freeze;
    public bool grappling;
    public bool activeGrapple;

    private void Start()
    {
        boss = FindObjectOfType<boss>();
        entityScript.Respawn();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {


        ResetLocation();
        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {   //  Mode - wallruning
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }
        //  Mode - swinging
        if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
        }
        

        if (freeze)
        {
            state = MovementState.freeze;
            desiredMoveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
                // Mode - Grappling
        else if (activeGrapple)
        {
            state = MovementState.grappling;
            moveSpeed = sprintSpeed;
        }
        // Mode - Swinging

        if (swinging)
        {
            state = MovementState.swinging;
            moveSpeed = swingSpeed; 
        }

        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        


        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }

        // check if desiredMoveSpeed has changed drastically
        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }
    public void ResetLocation()
    {
        playerPos = playerobj.position;
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName != "LOOPSCENE")
        {
        if(playerPos.y <= -60f)
        {
            if(!swinging)
            {
                if(!grappling)
                {
                    RespawnAllEntities();
                    boss.ReturnToOriginalPosition();
                    playerobj.position = new Vector3(0, 70, 0);
                }
            }
        }
        }
        else
        {
            if(playerPos.y <= -50f)
                {
                    if(!swinging)
                    {
                        if(!grappling)
                            {
                                playerobj.position = new Vector3(transform.position.x, 50, transform.position.z);
                            }
                    }
                }
        }


    }
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }
    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
    activeGrapple = true;

    

    velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight) 
                    * grappleSpeedMultiplier;

    Invoke(nameof(SetVelocity), 0.1f); 
    Invoke(nameof(StartDeceleration), 0.8f); 

    Invoke(nameof(ResetRestrictions), 1.5f); 




    }



private void StartDeceleration()
{
    StartCoroutine(SmoothDeceleration(1.4f)); 
}

private IEnumerator SmoothDeceleration(float duration)
{
    float time = 0;
    Vector3 initialVelocity = rb.velocity;
    float targetSpeed = grounded ? walkSpeed : 13f; 

    while (time < duration)
    {
        float progress = Mathf.SmoothStep(0f, 1f, time / duration); 
        Vector3 reducedVelocity = Vector3.Lerp(initialVelocity, initialVelocity.normalized * targetSpeed, progress);

        Vector3 moveInput = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.velocity = reducedVelocity + moveInput.normalized * moveSpeed * 0.5f;

        time += Time.deltaTime;
        yield return null;
    }
}



private void RespawnAllEntities()
{
    Debug.Log("d");
    Entity[] allEntities = FindObjectsOfType<Entity>(); 

    foreach (Entity entity in allEntities)
    {
        entity.Respawn();
    }
}
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }
    public void ResetRestrictions()
    {
        activeGrapple = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
        if (collision.gameObject.CompareTag("bossfight"))
        {
        boss.BossFightStart();            
        Debug.Log("BossFight :()");

        }

    if (collision.gameObject.layer == LayerMask.NameToLayer("Border"))
    {
 
        playerobj.position = new Vector3(0, 10, 0);
        Debug.Log("Player touched the border!");
        RespawnAllEntities();
        boss.ReturnToOriginalPosition();

    }
        if (collision.gameObject.CompareTag("sc1")) 
        {
            Debug.Log("wazaaaaaaap");
            
            LoadSceneByName("MAINMENU");

        }
                if (collision.gameObject.CompareTag("sc2")) 
        {
            Debug.Log("wazaaaaaaap");
            LoadSceneByName("MAINMENU");

        }
                if (collision.gameObject.CompareTag("sc0")) 
        {
            Debug.Log("wazaaaaaaap");
            LoadSceneByName("Tutorial");

        }

    }


        public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void MovePlayer()
    {
        if(activeGrapple) return;
        if(swinging) return;
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        if(!wallrunning)
        {rb.useGravity = !OnSlope();
        }
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }



    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0F, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            +Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}

