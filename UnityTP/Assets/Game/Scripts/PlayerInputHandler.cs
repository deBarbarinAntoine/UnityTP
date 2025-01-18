using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float speed = 7f;
    [SerializeField] private int jumpPower = 5;
    [SerializeField] private float lookSpeed = 100f;

    public Vector3 cameraOffset = new(0f, 0.5f, -0.5f); // Camera offset to see in 3rd person view
    public Vector3 aimingOffset = new(0f, 0f, -0.1f); // Camera offset for aiming (front of player)

    private Camera _camera;
    private InputAction _cameraAction;
    private InputAction _aimingAction;
    
    private float _cameraPitch;
    private float _cameraYaw;

    private bool _isGrounded;
    private InputAction _jumpAction;
    private InputAction _moveAction;

    private PlayerInputAction _myInputAction;
    private Rigidbody _rb;

    private bool _isAiming;
    
    // Initialization
    private void Awake()
    {
        _myInputAction = new PlayerInputAction();
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main; // >> make sure main camera is used for player <<

        // Set collision detection mode to "Continuous" to avoid passing through colliders
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Lock and hide the cursor when the game starts
        Cursor.lockState = CursorLockMode.Locked; // Locks the cursor to the center of the screen
        Cursor.visible = false; // Hides the cursor
    }

    // FixedUpdate is called once per frame (physical).
    private void FixedUpdate()
    {
        // Get the input movement direction
        var moveDir = _moveAction.ReadValue<Vector2>();

        // Get the camera's forward and right vectors
        var forward = _camera.transform.forward;
        var right = _camera.transform.right;

        // Make sure the forward vector only affects the x and z axis (ignore vertical component)
        forward.y = 0f;
        right.y = 0f;

        // Normalize the vectors to avoid faster diagonal movement
        forward.Normalize();
        right.Normalize();

        // Calculate the movement direction relative to the camera
        var movement = (forward * moveDir.y + right * moveDir.x) * speed;

        // Apply the movement to the rigidbody
        _rb.linearVelocity =
            new Vector3(movement.x, _rb.linearVelocity.y, movement.z); // Preserve y velocity (for gravity, jumps)
    }

    private void OnEnable()
    {
        _moveAction = _myInputAction.Player.Move;
        _moveAction.Enable();
        
        _jumpAction = _myInputAction.Player.Jump;
        _jumpAction.performed += OnJump;
        _jumpAction.Enable();
        
        _cameraAction = _myInputAction.Player.Camera;
        _cameraAction.performed += OnCameraAction;
        _cameraAction.Enable();
        
        _aimingAction = _myInputAction.Player.Aiming;
        _aimingAction.started += OnAimingStarted;
        _aimingAction.canceled += OnAimingCanceled;
        _aimingAction.Enable();
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _jumpAction.Disable();
        _cameraAction.Disable();
        _aimingAction.Disable();
    }

    private void OnCollisionExit()
    {
        _isGrounded = false;
    }

    private void OnCollisionStay()
    {
        _isGrounded = true;
    }

    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        if (_isGrounded) _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    private void OnCameraAction(InputAction.CallbackContext context)
    {
        // Get the mouse delta input for both axes (horizontal and vertical)
        var cameraInput = Mouse.current.delta.ReadValue();

        // Update the camera yaw and pitch based on the mouse input
        _cameraYaw += cameraInput.x * lookSpeed * Time.deltaTime; // Horizontal movement (yaw)
        _cameraPitch += cameraInput.y * lookSpeed * Time.deltaTime; // Vertical movement (pitch)

        // Clamp the pitch to avoid extreme angles
        _cameraPitch = Mathf.Clamp(_cameraPitch, -90f, 90f);

        // Determine the camera direction
        Vector3 direction = new Vector3(0f, 0f, _isAiming ? -aimingOffset.z : -cameraOffset.z);

        // Rotate around the player using yaw and pitch (camera rotation)
        var rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0);

        // Apply the rotation to the camera offset
        var rotatedOffset = rotation * direction;

        // Adjust the camera's position based on whether the player is aiming or not
        if (_isAiming)
        {
            // For aiming, position the camera using the aiming offset
            _camera.transform.position = transform.position + rotatedOffset;
        }
        else
        {
            // For normal movement, use the regular camera offset
            _camera.transform.position = transform.position + rotatedOffset;
        }

        // Make sure the camera always looks at the player or aiming point
        _camera.transform.LookAt(transform.position);

        // rotate the player's rigidbody
        var targetRotation = Quaternion.Euler(0, _cameraYaw, 0); // Only apply yaw rotation to the player
        _rb.MoveRotation(targetRotation);
    }

    
    private void OnAimingStarted(InputAction.CallbackContext context)
    {
        _isAiming = true;
        OnCameraAction(context);
    }

    private void OnAimingCanceled(InputAction.CallbackContext context)
    {
        _isAiming = false;
        OnCameraAction(context);
    }
}