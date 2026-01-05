using UnityEngine;

public class PackagePickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private PackagePool _pool;
    private GameManager _gm;

    public void Init(PackagePool pool, GameManager gm)
    {
        _pool = pool;
        _gm = gm;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (_gm != null)
            _gm.AddPackage();

        if (_pool != null)
            _pool.Return(gameObject);
        else
            gameObject.SetActive(false);
    }
}
