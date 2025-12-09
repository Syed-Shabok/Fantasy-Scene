using System.Collections;
using UnityEngine;

public class UpdatedPlayerControlScript : MonoBehaviour
{
    public float baseSpeed = 1.0f;
    public float rotationSpeed = 80f;
    public float jumpForce = 5.0f;
    public float runMultiplier = 2.0f; // Factor to increase speed while running

    public AudioClip jumpSound;
    public float jumpVolume = 1.0f;

    public Transform playerCamera;
    public float mouseSensitivity = 100.0f;

    private Rigidbody rb;
    private AudioSource audioSource;
    private float xRotation = 0f;

    public Animator playerAnim;
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isGrounded = true;
    private float currentSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        Cursor.lockState = CursorLockMode.Locked;

        currentSpeed = baseSpeed; // Initialize the current speed to base walking speed
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        HandleCameraRotation();
    }

    private void MovePlayer()
    {
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * moveVertical * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void RotatePlayer()
    {
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SetMovementState(true, false, "WalkForward");
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            SetMovementState(false, false, "Idle");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SetMovementState(true, false, "WalkBackward");
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            SetMovementState(false, false, "Idle");
        }

        if (isWalking && Input.GetKeyDown(KeyCode.LeftShift))
        {
            SetMovementState(true, true, "RunForward");
        }
        else if (isRunning && Input.GetKeyUp(KeyCode.LeftShift))
        {
            SetMovementState(true, false, "WalkForward");
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            playerAnim.SetTrigger("JumpUp");

            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, jumpVolume);
            }

            Invoke(nameof(PlayJumpEndAnimation), 0.7f);
        }
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void SetMovementState(bool walking, bool running, string animationTrigger)
    {
        isWalking = walking;
        isRunning = running;
        currentSpeed = running ? baseSpeed * runMultiplier : baseSpeed;

        playerAnim.SetTrigger(animationTrigger);
        playerAnim.ResetTrigger("Idle");

        if (!walking && !running)
        {
            playerAnim.SetTrigger("Idle");
        }
    }

    private void PlayJumpEndAnimation()
    {
        isGrounded = true;

        if (isRunning)
        {
            playerAnim.SetTrigger("RunForward");
        }
        else if (isWalking)
        {
            playerAnim.SetTrigger("WalkForward");
        }
        else
        {
            playerAnim.SetTrigger("Idle");
        }

        playerAnim.ResetTrigger("JumpUp");
    }
}
