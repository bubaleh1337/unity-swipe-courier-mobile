using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("[Game] LOSE");
            Time.timeScale = 0f; // заморозка игры на прототипе
        }
    }
}
