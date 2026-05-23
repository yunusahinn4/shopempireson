using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;

    public Transform spawnPoint;

    public float spawnInterval = 3f;

    void Start()
    {
        InvokeRepeating(
            nameof(SpawnCustomer),
            1f,
            spawnInterval
        );
    }

    void SpawnCustomer()
    {
        Instantiate(
            customerPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }
}