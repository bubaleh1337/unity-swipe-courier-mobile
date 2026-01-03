using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 10f;
    public float destroyZ = -8f;

    private ObstaclePool _pool;

    public void Init(ObstaclePool pool, float moveSpeed)
    {
        _pool = pool;
        speed = moveSpeed;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += Vector3.back * (speed * Time.deltaTime);

        if (transform.position.z < destroyZ)
        {
            if (_pool != null) _pool.Release(this);
            else gameObject.SetActive(false);
        }
    }
}
