using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public GameObject painPrefab; // R�f�rence au prefab de la tranche de pain
    public Transform spawnPoint; // Point de sortie des munitions
    public float shootForce = 5f; // Force du projecrile
    public float fireRate = 0.5f; // Temps de wait entre deux tirs (en secondes)


    private PlayerInputAction _myInputAction;

    private float _nextFireTime; // Chrono cadence de tir
    private int _painCount; // Compteur pour le nombre de pains tir�s
    private InputAction _shootAction;


    // Initialization
    private void Awake()
    {
        _myInputAction = new PlayerInputAction();
    }

    private void OnEnable()
    {
        _shootAction = _myInputAction.Player.Fire;
        _shootAction.performed += Shoot;
        _shootAction.Enable();
    }

    private void OnDisable()
    {
        _shootAction.Disable();
    }

    private void Shoot(InputAction.CallbackContext callbackContext)
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