using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public GameObject painPrefab; // Reference to the bread prefab
    public Transform spawnPoint; // Spawn point for the projectiles
    public float shootForce = 5f; // Force of the projectile
    public float fireRate = 0.5f; // Time between shots (in seconds)

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
        // Check if enough time has passed before shooting again
        if (Time.time < _nextFireTime) return; // If the cooldown hasn't passed, don't shoot

        // Update the next fire time based on the current time and the fire rate
        _nextFireTime = Time.time + fireRate;

        // Create an instance of the bread prefab
        var painInstance = Instantiate(painPrefab, spawnPoint.position, spawnPoint.rotation);

        // Rotate the prefab 180° along the X-axis (if needed)
        painInstance.transform.Rotate(180f, 0f, 0f);

        // Scale down the bread object
        painInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Add the BreadCollision script to handle collision
        painInstance.AddComponent<BreadCollision>();

        // Apply force to propel the bread in the opposite direction
        var rb = painInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            var shootDirection = -transform.forward; // Shooting in the opposite direction
            rb.AddForce(shootDirection.normalized * shootForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Le prefab du pain n'a pas de Rigidbody!");
        }

        // Increments the shot counter and logs the information
        _painCount++;

        // Destroy the bread after 5 seconds to avoid accumulation
        Destroy(painInstance, 5f);
    }
}