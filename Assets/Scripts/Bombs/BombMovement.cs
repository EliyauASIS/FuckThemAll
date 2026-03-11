using UnityEngine;

public class BombMovement : MonoBehaviour
{
    private float _speed;
    private RectTransform _rt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
    }

    public void Init(float speed)
    {
        _speed = speed;
    }

    private void Update()
    {
        _rt.anchoredPosition += Vector2.down * _speed * Time.deltaTime;
    }
}