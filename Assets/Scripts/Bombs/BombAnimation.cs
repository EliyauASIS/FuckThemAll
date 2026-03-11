using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BombAnimation : MonoBehaviour
{
    [Header("Frames")]
    public Sprite[] frames;
    [SerializeField] private float frameInterval = 0.2f;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        int i = 0;
        while (true)
        {
            _image.sprite = frames[i % frames.Length];
            i++;
            yield return new WaitForSeconds(frameInterval);
        }
    }
}