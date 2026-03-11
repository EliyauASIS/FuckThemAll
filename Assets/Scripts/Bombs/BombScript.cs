using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] private string bulletTag = "Bullet";
    [SerializeField] private GameObject groundObject;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject explosionHandler;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private GameObject shootPrefab;
    [SerializeField] private GameObject shootHandler;
    [SerializeField] private AudioSource shootSound;
    private RectTransform _rt;
    private RectTransform _canvasRt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _canvasRt = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        groundObject = GameObject.Find("Ground");
        explosionHandler = GameObject.Find("Explosions");
        explosionSound = GameObject.Find("Explosion Audio Source").GetComponent<AudioSource>();

        shootHandler = GameObject.Find("Shoots");
        shootSound = GameObject.Find("Bomb Destroy Audio Source").GetComponent<AudioSource>();
    }

    private void Update()
    {
        CheckGroundHit();
        CheckBulletHit();
    }

    private void CheckGroundHit()
    {
        var groundRt = groundObject.GetComponent<RectTransform>();
        if (groundRt == null) { return; }

        if (RectOverlaps(_rt, groundRt))
        {
            Debug.Log("[BombScript] פצצה נגעה ברצפה!");
            GameManager.instance.LoseLife();
            SpawnExplosion();
            Destroy(gameObject);
            return;
        }
    }
    private void CheckBulletHit()
    {
        var bullets = GameObject.FindGameObjectsWithTag(bulletTag);

        foreach (var bullet in bullets)
        {
            var bulletRt = bullet.GetComponent<RectTransform>();
            if (bulletRt == null) continue;

            if (RectOverlaps(_rt, bulletRt))
            {
                PointsManager.instance.UpdatePoints(10);
                GameManager.instance.bombDestroyed++;
                SpawnShoot();
                Destroy(bullet);
                Destroy(gameObject);
                return;
            }
        }
    }
    private void SpawnShoot()
    {
        if (shootPrefab == null) return;

        GameObject shootInstance = Instantiate(shootPrefab, shootHandler.transform);
        var rt = shootInstance.GetComponent<RectTransform>();
        Vector3 pos = _rt.position + (Vector3)_rt.rect.center;
        pos.y -= 100f;
        rt.position = pos;
        shootInstance.GetComponent<UIAnimation>().StartAnimationCoroutine();

        shootSound.Play();
    }
    private void SpawnExplosion()
    {
        if (explosionPrefab == null) return;

        GameObject explosionInstance = Instantiate(explosionPrefab, explosionHandler.transform);
        explosionInstance.GetComponent<RectTransform>().position = _rt.position;
        explosionInstance.GetComponent<UIAnimation>().StartAnimationCoroutine();

        explosionSound.Play();
    }
    private bool RectOverlaps(RectTransform a, RectTransform b)
    {
        var cornersA = new Vector3[4];
        var cornersB = new Vector3[4];

        a.GetWorldCorners(cornersA);
        b.GetWorldCorners(cornersB);

        var rectA = new Rect(cornersA[0].x, cornersA[0].y, cornersA[2].x - cornersA[0].x, cornersA[2].y - cornersA[0].y);
        var rectB = new Rect(cornersB[0].x, cornersB[0].y, cornersB[2].x - cornersB[0].x, cornersB[2].y - cornersB[0].y);

        return rectA.Overlaps(rectB);
    }
}