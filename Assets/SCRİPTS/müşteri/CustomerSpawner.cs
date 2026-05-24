using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnData
    {
        public Transform spawnPoint;

        // 1 = saða gider, -1 = sola gider
        public int direction = 1;
    }

    public GameObject customerPrefab;
    public SpawnData[] spawnPoints;

    public float spawnInterval = 8f;
    public float fixedY = -3.6f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCustomer), 1f, spawnInterval);
    }

    void SpawnCustomer()
    {
        if (spawnPoints.Length == 0)
            return;

        SpawnData selected = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 spawnPos = selected.spawnPoint.position;
        spawnPos.y = fixedY;

        GameObject customer = Instantiate(customerPrefab, spawnPos, Quaternion.identity);

        CustomerMovement movement = customer.GetComponent<CustomerMovement>();

        if (movement != null)
        {
            movement.SetDirection(selected.direction);
        }
    }
}