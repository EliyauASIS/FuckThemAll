using System.Collections;
using UnityEngine;
using TrackerPro.Unity.HandTracking;

public class CannonScript : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletsHolder;
    public BulletScript cannonBulletPrefab;
    public RectTransform spawnPosition;
    public HandTracker handTracker;
    public AudioSource spawnSound;

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.3f; // שניות בין כל יריה

    private Coroutine _fireCoroutine;

    private void OnEnable()
    {
        handTracker.OnGestureDetected += StartFiring;
        handTracker.OnGestureEnded += StopFiring;
    }

    private void OnDisable()
    {
        handTracker.OnGestureDetected -= StartFiring;
        handTracker.OnGestureEnded -= StopFiring;
    }

    private void StartFiring()
    {
        if (_fireCoroutine != null)
            StopCoroutine(_fireCoroutine);
        _fireCoroutine = StartCoroutine(FireLoop());
    }

    private void StopFiring()
    {
        if (_fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
    }

    private IEnumerator FireLoop()
    {
        while (true)
        {
            ShotBullet();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void ShotBullet()
    {
        if (GameManager.instance.gameOver)
        {
            return;
        }
        var bullet = Instantiate(cannonBulletPrefab, bulletsHolder.transform);
        bullet.GetComponent<RectTransform>().position = spawnPosition.position;
        bullet.Init(spawnPosition.up);

        spawnSound.Play();
    }
}