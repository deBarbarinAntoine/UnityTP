using UnityEngine;

public class BreadCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Destroy the bread (painInstance) when it collides with something
        Destroy(gameObject);
    }
}