using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public GameObject painPrefab; // Reference to the bread prefab
    public Transform spawnPoint; // Spawn point for the projectiles
    public float shootForce = 5f; // Force of the projectile
    public float fireRate = 0.5f; // Time between shots (in seconds)

    private InputAction _aimAction;
    private Vector3 _aimDirection; // Direction to shoot based on camera view
    private bool _isAiming; // Track if the player is holding down the aiming button

    private bool _isShooting; // Track if the player is holding down the shoot button

    private PlayerInputAction _myInputAction;
    private float _nextFireTime; // Timer for fire rate
    private int _painCount; // Counter for the number of breads shot
    private InputAction _shootAction;

    // Initialization
    private void Awake()
    {
        _myInputAction = new PlayerInputAction();
    }

    private void FixedUpdate()
    {
        // Keep shooting while the button is held down and enough time has passed between shots
        if (_isShooting && Time.time >= _nextFireTime) Shoot();
    }

    private void OnEnable()
    {
        _shootAction = _myInputAction.Player.Fire;
        _shootAction.started += StartShooting; // Start shooting when the button is pressed
        _shootAction.canceled += StopShooting; // Stop shooting when the button is released
        _shootAction.Enable();
    }

    private void OnDisable()
    {
        _shootAction.Disable();
        _aimAction.Disable();
    }

    // Start shooting when button is pressed
    private void StartShooting(InputAction.CallbackContext context)
    {
        _isShooting = true;
    }

    // Stop shooting when button is released
    private void StopShooting(InputAction.CallbackContext context)
    {
        _isShooting = false;
    }

    private void Shoot()
    {
        // Check if enough time has passed before shooting again (fire rate control)
        if (Time.time < _nextFireTime) return;

        // Update the next fire time based on the current time and the fire rate
        _nextFireTime = Time.time + fireRate;

        // Create an instance of the bread prefab
        var painInstance = Instantiate(painPrefab, spawnPoint.position, spawnPoint.rotation);

        // Get the direction from the prefab to the camera
        var directionToCamera = Camera.main.transform.rotation;
        
        // apply an additional rotation to the prefab
        painInstance.transform.rotation = directionToCamera;

        // Scale down the bread object
        painInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Add the BreadCollision script to handle collision
        painInstance.AddComponent<BreadCollision>();

        // Apply force to propel the bread in the calculated direction (3D)
        var rb = painInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (Camera.main)
            {
                var cameraForward = Camera.main.transform.forward;
                // Normalize the direction to avoid unintended scale due to angles
                var shootDirection = cameraForward.normalized;

                rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Camera not found");
            }
        }
        else
        {
            Debug.LogError("The bread prefab does not have a Rigidbody component!");
        }

        // Increment the shot counter and log the information
        _painCount++;

        // Destroy the bread after 5 seconds to avoid accumulation
        Destroy(painInstance, 5f);
    }
}