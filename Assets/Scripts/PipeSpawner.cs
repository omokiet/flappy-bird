using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float spawnInterval = 2f;
    public float heightRange = 0.5f;

    private bool hasStarted = false;

    public void StartSpawning()
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;
        InvokeRepeating(nameof(SpawnPipe), 0f, spawnInterval);
    }

    private void SpawnPipe()
    {
        float offsetY = Random.Range(-heightRange, heightRange);

        Vector3 spawnPosition = transform.position;
        spawnPosition.y += offsetY;

        GameObject newPipe = Instantiate(
            pipePrefab,
            spawnPosition,
            Quaternion.identity
        );

        Destroy(newPipe, 10f);
    }
}