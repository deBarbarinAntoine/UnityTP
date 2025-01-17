using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject painPrefab; // R�f�rence au prefab de la tranche de pain
    public Transform spawnPoint; // Point de sortie des munitions
    public float shootForce = 5f; // Force du projecrile
    public float fireRate = 0.5f; // Temps de wait entre deux tirs (en secondes)

    private float _nextFireTime = 0f; // Chrono cadence de tir
    private int _painCount = 0; // Compteur pour le nombre de pains tir�s

    private void Update()
    {
        // V�rifie si le joueur appuie sur la touche "Espace" et que le temps entre les tirs est respect�
        if (Input.GetKey(KeyCode.Space) && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate; // D�termine le prochain moment o� on peut tirer
        }
    }

    private void Shoot()
    {
        if (painPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Pain prefab ou spawnPoint non assign�!");
            return;
        }

        // Cr�e une instance du prefab
        GameObject painInstance = Instantiate(painPrefab, spawnPoint.position, spawnPoint.rotation);

        // Applique une force pour propulser le pain
        Rigidbody rb = painInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Force principale vers l'avant + un peu de force vers le haut
            Vector3 shootDirection = spawnPoint.forward + (Vector3.up * 0.1f); // R�duit la composante verticale
            rb.AddForce(shootDirection.normalized * shootForce, ForceMode.Impulse);

            // Ajoute un peu de rotation al�atoire
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

            // Log de la direction de tir
            Debug.Log("Direction de tir : " + shootDirection);
        }
        else
        {
            Debug.LogError("Le prefab du pain n'a pas de Rigidbody!");
        }

        // Incr�mente le compteur de pains tir�s
        _painCount++;

        // Affiche le nombre de pains tir�s dans la console
        Debug.Log("Pain tir� n� " + _painCount + " depuis : " + spawnPoint.position);

        // D�truit le pain apr�s 5 secondes pour �viter l'accumulation
        Destroy(painInstance, 5f);
    }

//POUR VOIR LA DIRECTION DU PAIN
    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawnPoint.position, 0.1f);
            Gizmos.DrawRay(spawnPoint.position, spawnPoint.forward * 2f);
        }
    }
}
