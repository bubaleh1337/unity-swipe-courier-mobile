using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager _gm;

    private void Awake()
    {
        _gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            _gm?.Lose();
        }
    }
}
