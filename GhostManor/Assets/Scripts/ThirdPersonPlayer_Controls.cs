using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ThirdPersonPlayer_Controls : MonoBehaviour
{
    // input feilds
    // Reference to the gerated input Action C# script
    private GhostInputActions ghostInputActions;
    private InputAction movement;

    // movement feilds
    private Rigidbody rb;
    private new Collider collider;
    private Transform playerTrans;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;
    private float distToGround;

    [SerializeField]
    private Camera playerCamera;
    private float x, y;

    [SerializeField]
    GameObject capturedPlayer;
    // any number above 0 for inVision means player is in vision cone
    [SerializeField]
    private int VisionCount; // player in vision
    [SerializeField]
    private float captureDistance = 5f;
    [SerializeField]
    private float captureTime = 2.5f; // time in sec. it takes to capture
    private float timeRemaining = 0f;
    private bool timerIsRunning = false;
    private GameObject capturedEnemy;

    //set mask to the mask of the object you
    //want to look at in the editor.
    public LayerMask mask;

    private void Start()
    {
        // get the distance to ground
        distToGround = collider.bounds.extents.y;
    }

    private void Update()
    {
        x = Screen.width / 2;
        y = Screen.height / 2;

        Timer();
    }

    private void Timer()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                // Time ran out
                timeRemaining = 0;
                timerIsRunning = false;

                if (VisionCount <= 0)
                {
                    StartCoroutine(CaptureEnemy());
                }
            }
        }
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // cache ref.
        rb = this.GetComponent<Rigidbody>();
        collider = this.GetComponent<CapsuleCollider>();
        playerTrans = this.GetComponent<Transform>();

        // Input action asset is not static or gloabal
        // So we must make a new instance of the input action asset
        ghostInputActions = new GhostInputActions();
    }

    private void OnEnable()
    {
        // cache reference of the move variable
        movement = ghostInputActions.Player.Move;
        // We NEED to enable the movement
        movement.Enable();

        // Subsribe a function to the perform event
        // on the jump input action
        ghostInputActions.Player.Jump.performed += DoJump;
        ghostInputActions.Player.Jump.Enable();

        ghostInputActions.Player.Fire.performed += DoFire;
        ghostInputActions.Player.Fire.Enable(); 
    }

    // Reason for OnDisable(): events wont get called and
    // thus throw errors if the object is disabled
    private void OnDisable()
    {
        movement.Disable();
        ghostInputActions.Player.Jump.Disable();
        ghostInputActions.Player.Fire.Disable();
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        //Debug.Log("Jump!!!");
        if (isGrounded())
        {
            //Debug.Log("Jump Force!!!");
            forceDirection += Vector3.up * jumpForce;
        }
    }

    // Raycast to check if hit enemy
    private void DoFire(InputAction.CallbackContext obj)
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(x, y, 0));
        //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow, 10.0f);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("BasicEnemy"))
            {
                GameObject hitEnemy = hit.collider.gameObject;

                if (Vector3.Distance(playerTrans.position, hitEnemy.transform.position) < captureDistance) 
                {
                    //CinemachineVirtualCamera hitCam = hitEnemy.GetComponentInChildren<CinemachineVirtualCamera>();
                    //hitCam.Priority = 20;

                    //hitEnemy.GetComponent<BasicEnemy_Controller>().RemoveEnemy();
                    if (VisionCount <= 0)
                    {
                        capturedEnemy = hitEnemy;

                        // enables timer in update method
                        timeRemaining = captureTime;
                        timerIsRunning = true;
                    }
                }
            }
        }
    }

    IEnumerator CaptureEnemy()
    {
        // GameObject capturedEnemy : Enemy that got captured

        CinemachineVirtualCamera hitCam = capturedEnemy.GetComponentInChildren<CinemachineVirtualCamera>();
        hitCam.Priority = 20;

        
        float dist = Vector3.Distance(hitCam.transform.position, capturedEnemy.transform.position);
        // wait until close enough to continue
        yield return new WaitUntil(() => dist < 0.5f);

        Transform capturedEnemyTrans = capturedEnemy.GetComponent<Transform>();
        capturedEnemy.GetComponent<BasicEnemy_Controller>().RemoveEnemy();
        
        this.enabled = false;
        capturedPlayer.transform.position = capturedEnemyTrans.position;
        capturedPlayer.SetActive(true);
    }

    public void ChangeVisionCount(int count)
    {
        VisionCount += count;
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, (float)(distToGround + 0.1));
    }

    private void FixedUpdate()
    {
        // Debug.Log("Movement Values " + movement.ReadValue<Vector2>());
        // Why not use transform.right? we want local movement
        forceDirection += movement.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += movement.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        // Apply force to the rigid body
        rb.AddForce(forceDirection, ForceMode.Impulse);
        // once they let go of WASD the play will stop
        forceDirection = Vector3.zero;

        // accelerate the player downwards to remove float-iness
        if (rb.velocity.y < 0f)
        {
            // increase acceleration as as we fall
            // rb.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
            rb.velocity += Vector3.down * Time.fixedDeltaTime;
        }

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        LookAt();
    }

    // Rotate player in the correct diraction
    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        // only rotate left and right (or y axis)
        direction.y = 0f;

        if (movement.ReadValue<Vector2>().sqrMagnitude > 0.1 && direction.sqrMagnitude > 0.1f)
        {
            // control rotation of the rigid body
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            // stop rotating if there is no input
            rb.angularVelocity = Vector3.zero;
        }
    }

    // the method "GetCamera" Forward and Right is used to not  
    // change player camera speed based on the angle of the camera
    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        // normalized because once we take off that verical component
        // the length of the vector is no longer 1
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        // normalized because once we take off that verical component
        // the length of the vector is no longer 1
        return right.normalized;
    }
}