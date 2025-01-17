using UnityEngine;

public class ShooterBlockController : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign the Projectile prefab in the inspector
    public Transform shootingPoint;     // Assign the Shooting Point transform in the inspector
    public float shootForce = 500f;     // Adjust the force for shooting

    void FixedUpdate()
    {
        // Check for input (Space key or any other key for shooting)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Instantiate the projectile at the shooting point
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, shootingPoint.rotation);

        // Add force to the projectile to make it move
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(shootingPoint.forward * shootForce);
        }
    }
}