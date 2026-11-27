using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public float moveSpeed = 2f;

    private void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }
}