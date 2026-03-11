using System.Collections;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject[] cloudsPrefabs;
    public RectTransform spawnArea;

    [Header("Settings")]
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 7f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            SpawnCloud();
        }
    }

    private void SpawnCloud()
    {
        var prefab = cloudsPrefabs[Random.Range(0, cloudsPrefabs.Length)];
        var cloud = Instantiate(prefab, spawnArea);
        var rt = cloud.GetComponent<RectTransform>();

        float halfWidth = spawnArea.rect.width / 2f;
        float halfHeight = spawnArea.rect.height / 2f;
        rt.anchoredPosition = new Vector2(
            Random.Range(-halfWidth, halfWidth),
            Random.Range(-halfHeight, halfHeight)
        );
    }
}