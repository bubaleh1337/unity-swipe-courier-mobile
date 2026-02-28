using UnityEngine;

public class DeliveryZoneTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (gameManager != null)
            gameManager.TryCompleteDeliveryAtZone();
    }
}
