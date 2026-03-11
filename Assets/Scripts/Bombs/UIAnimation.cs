using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [Header("Frames")]
    public Sprite[] frames;
    [SerializeField] private float frameInterval = 0.2f;

    private Image _image;

    
    public void StartAnimationCoroutine()
    {
        _image = GetComponent<Image>();
        StartCoroutine(PlayAnimation());      
    }

    private IEnumerator PlayAnimation()
    {
        foreach (var frame in frames)
        {
            _image.sprite = frame;
            yield return new WaitForSeconds(frameInterval);
        }

        Destroy(gameObject);
    }
}