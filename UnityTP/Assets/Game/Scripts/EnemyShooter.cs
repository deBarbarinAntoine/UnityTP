using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Référence au prefab de l'objet à lancer
    public Transform spawnPoint; // Point de sortie des munitions
    public float shootForce = 15f; // Force de projection
    public float fireRate = 2f; // Temps entre deux tirs  en s
    public Transform player; // cible joueur

    private float nextFireTime = 0f; //  cadence de tir

    private void Update()
    {
        if (player != null && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate; // Détermine le prochain moment où on peut tirer
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || spawnPoint == null)
        {
            Debug.LogError("Projectile prefab ou spawnPoint non assigné !");
            return;
        }

        //  instance du prefab
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);

        // force pour propulser le projectile vers le joueur
        Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calcule la direction vers le joueur
            Vector3 shootDirection = (player.position - spawnPoint.position).normalized;
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
            Debug.LogError("Projectile lancé");
        }
        else
        {
            Debug.LogError("Le prefab du projectile n'a pas de Rigidbody !");
        }

        // Détruit le projectile après 5 secondes pour éviter l'accumulation
        Destroy(projectileInstance, 5f);
    }


    //private void OnDrawGizmos()
    //{
    //    if (spawnPoint != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawSphere(spawnPoint.position, 0.1f); // Point de spawn

    //        // Dessine une ligne pour représenter la direction de tir
    //        Vector3 shootDirection = spawnPoint.forward * 2f; // Longueur de la ligne
    //        Gizmos.DrawRay(spawnPoint.position, shootDirection); // Direction de tir

    //        // Optionnel : dessiner la trajectoire du projectile
    //        Vector3 trajectoryEnd = spawnPoint.position + shootDirection; // Point final de la trajectoire
    //        Gizmos.color = Color.blue; // Couleur de la trajectoire
    //        Gizmos.DrawLine(spawnPoint.position, trajectoryEnd); // Trajectoire
    //    }
    //}
}