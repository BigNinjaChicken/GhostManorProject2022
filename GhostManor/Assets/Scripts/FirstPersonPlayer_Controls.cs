using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class FirstPersonPlayer_Controls : MonoBehaviour
{
    // input feilds
    // Reference to the gerated input Action C# script
    private GhostInputActions ghostInputActions;
    private InputAction movement;
    private InputAction look;

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
    [SerializeField]
    private CinemachineVirtualCamera virtualCam;
    private float x, y;

    [SerializeField]
    private float sensitivityX = 8f;
    [SerializeField]
    private float sensitivityY = 0.5f;
    private float mouseX, mouseY;
    [SerializeField]
    private float xClamp = 85f;
    private float xRotation = 0f;

    [SerializeField] private RectTransform healthUI;

    private float maxHealth = 100f;
    public float currentHealth;

    [SerializeField] private float decayMultiplier = 2f;

    [SerializeField] private GameObject ghostPlayer;
    [SerializeField] private GameObject ghostPlayerMesh;

    private void Start()
    {
        currentHealth = maxHealth;

        // get the distance to ground
        distToGround = collider.bounds.extents.y;
    }

    private void Update()
    {
        currentHealth -= (Time.deltaTime * decayMultiplier);
        healthUI.localScale = new Vector3(currentHealth / 100, 1, 1);

        if (currentHealth <= 0)
        {
            ghostPlayer.transform.position = transform.position;
            ghostPlayer.transform.rotation = transform.rotation;
            ghostPlayer.GetComponent<ThirdPersonPlayer_Controls>().enabled = true;
            ghostPlayerMesh.SetActive(true);
            gameObject.SetActive(false);
        }

        x = Screen.width / 2;
        y = Screen.height / 2;
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
        look = ghostInputActions.Player.Look;
        // We NEED to enable the movement
        movement.Enable();
        look.Enable();

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
        look.Disable();
        ghostInputActions.Player.Jump.Disable();
        ghostInputActions.Player.Fire.Disable();
    }

    private void FixedUpdate()
    {
        // Debug.Log("Movement Values " + movement.ReadValue<Vector2>());
        // Why not use transform.right? we want local movement
        // movement.ReadValue<Vector2>().x, movement.ReadValue<Vector2>().y
        // rb.AddForce(forceDirection, ForceMode.Impulse);

        forceDirection.x += movement.ReadValue<Vector2>().x * movementForce;
        forceDirection.z += movement.ReadValue<Vector2>().y * movementForce;

        // Apply force to the rigid body
        rb.AddRelativeForce(forceDirection, ForceMode.Impulse);
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

        // Rotate Camera
        mouseX = look.ReadValue<Vector2>().x * sensitivityX;
        mouseY = look.ReadValue<Vector2>().y * sensitivityY;

        transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        virtualCam.transform.eulerAngles = targetRotation;
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, (float)(distToGround + 0.1));
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

                
                    //CinemachineVirtualCamera hitCam = hitEnemy.GetComponentInChildren<CinemachineVirtualCamera>();
                    //hitCam.Priority = 20;

                    //hitEnemy.GetComponent<BasicEnemy_Controller>().RemoveEnemy();
                    
                
            }
        }
    }
}
