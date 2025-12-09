using UnityEngine;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f; // Set player's movement speed.
    public float rotationSpeed = 120.0f; // Set player's rotation speed.
    public float jumpForce = 5.0f; // Set player's jump force.

    public AudioClip jumpSound; // Audio clip to play when jumping.
    public float jumpVolume = 1.0f; // Set the volume of the jump sound

    public Transform playerCamera; // Reference to the player camera object
    public float mouseSensitivity = 100.0f; // Sensitivity for mouse movement

    private Rigidbody rb; // Reference to player's Rigidbody.
    private AudioSource audioSource; // AudioSource to play sound effects.
    private float xRotation = 0f; // Track vertical rotation

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Access player's Rigidbody.

        // Add an AudioSource component if one doesn't exist.
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // Ensure the sound doesn't play automatically.

        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to center of screen
    }

    // Update is called once per frame
    void Update()
    {
        // Handle jump
        if (Input.GetButtonDown("Jump")) 
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            // Play the jump sound with specified volume if the audio clip is assigned.
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound, jumpVolume);
            }
        }

        // Handle camera rotation based on mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limit vertical rotation to avoid flipping

        // Rotate the camera and player based on mouse input
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // Handle physics-based movement and rotation.
    private void FixedUpdate()
    {
        // Move player based on vertical input.
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = transform.forward * moveVertical * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate player based on horizontal input.
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
