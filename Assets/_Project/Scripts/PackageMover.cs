using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PackageMover : MonoBehaviour
{
    private PackagePool _pool;
    private GameManager _gm;

    private Rigidbody _rb;
    private float _speed;
    private float _despawnZ;

    public void Init(PackagePool pool, GameManager gm, float speed, float despawnZ)
    {
        _pool = pool;
        _gm = gm;
        _speed = speed;
        _despawnZ = despawnZ;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (_gm != null && !_gm.IsRunning) return;
        if (Time.timeScale == 0f) return;

        Vector3 pos = _rb.position;
        pos.z -= _speed * Time.fixedDeltaTime;
        _rb.MovePosition(pos);

        if (pos.z < _despawnZ)
        {
            if (_pool != null) _pool.Return(gameObject);
            else gameObject.SetActive(false);
        }
    }
}
