using System.Collections;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{

    public float minSpeed;
    public float maxSpeed;
    private RectTransform _rt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        StartCoroutine(DestroyDuration());
    }

    private void Update()
    {
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        _rt.anchoredPosition += Vector2.left * randomSpeed * Time.deltaTime;
    }

    private IEnumerator DestroyDuration()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}