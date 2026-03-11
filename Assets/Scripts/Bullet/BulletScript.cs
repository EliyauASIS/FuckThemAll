using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float speed = 800f;

    private RectTransform _rt;
    private Vector3 _direction;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void Init(Vector3 direction)
    {
        _direction = direction;
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        _rt.position += _direction * speed * Time.deltaTime;
    }
}