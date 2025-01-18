using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Reference to the projectile prefab
    public Transform spawnPoint; // Point from where projectiles will be spawned
    public float shootForce = 5f; // Force applied to the projectile
    public float fireRate = 2f; // Time between two shots in seconds
    public Transform player; // The player's position (target)
    public float shootingRange = 10f; // Maximum distance at which the enemy can shoot

    private float _nextFireTime; // Time for the next shot

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the player is within the shooting range and if enough time has passed for the next shot
            if (distanceToPlayer <= shootingRange && Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + fireRate; // Set the next available fire time
            }
        }
    }

    private void Shoot()
    {
        // Instantiate the projectile prefab
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        // Get the Rigidbody component to apply force
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        // Add BreadCollision script to handle collision
        projectileInstance.AddComponent<BreadCollision>();

        if (rb != null)
        {
            // Calculate the direction to the player and apply force
            Vector3 shootDirection = (player.position - spawnPoint.position).normalized;
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Rigidbody!");
        }

        // Destroy the projectile after 5 seconds to avoid accumulation
        Destroy(projectileInstance, 5f);
    }
}
