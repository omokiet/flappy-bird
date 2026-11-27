using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BirdController bird = other.GetComponent<BirdController>();

        if (bird != null)
        {
            bird.AddScore();
        }
    }
}
