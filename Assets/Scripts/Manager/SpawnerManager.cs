using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerManager : MonoBehaviour
{
    public static SpawnerManager instance;

    [Header("References")]
    public Image[] bombPrefabs;
    public RectTransform spawnArea; // Canvas או Panel שממנו נוצרים הטילים
    Coroutine spawnCoroutine;

    [Header("Settings")]
    [SerializeField] private float minInterval = 2f;
    [SerializeField] private float maxInterval = 5f;
    [SerializeField] private float minFallSpeed = 300f;
    [SerializeField] private float maxFallSpeed = 600f;

    private void Awake()
    {
        instance = this;
    }
    public void StartSpawn()
    {
        spawnCoroutine = StartCoroutine(SpawnLoop());
    }
    public void Stop()
    {
        StopCoroutine(spawnCoroutine);
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("Bomb");
        foreach (GameObject bomb in bombs)
        {
            Destroy(bomb);
        }
    }
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            SpawnMissile();
        }
    }

    private void SpawnMissile()
    {
        var prefab = bombPrefabs[Random.Range(0, bombPrefabs.Length)];

        var missile = Instantiate(prefab, spawnArea);
        var rt = missile.GetComponent<RectTransform>();

        // מציב בX רנדומלי בחלק העליון של ה-spawnArea
        float halfWidth = spawnArea.rect.width / 2f;
        rt.anchoredPosition = new Vector2(Random.Range(-halfWidth, halfWidth), spawnArea.rect.height / 2f);

        missile.gameObject.AddComponent<BombMovement>().Init(Random.Range(minFallSpeed,maxFallSpeed));
    }
}