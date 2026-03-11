using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrackerPro.Unity.HandTracking;
using TrackerPro.Unity;
using TrackerPro;

public class CannonMovement : MonoBehaviour
{
    [Header("References")]
    public HandTrackingGraph graphRunner;
    private RectTransform _rectTransform;

    [Header("Rotation")]
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float smoothSpeed = 10f;

    private float _handX = 0.5f;
    private float _currentAngle = 0f;
    private bool _subscribed = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        float elapsed = 0f;
        while (!_subscribed && elapsed < 10f)
        {
            try
            {
                graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
                _subscribed = true;
            }
            catch { }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (graphRunner != null && _subscribed)
            graphRunner.OnHandLandmarksOutput -= OnHandLandmarksOutput;
    }

    private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
    {
        var hands = eventArgs.value;
        if (hands == null || hands.Count == 0) return;

        // ממוצע של כל נקודות השלד - יציב יותר מנקודה בודדת
        float sum = 0f;
        var landmarks = hands[0].Landmark;
        foreach (var lm in landmarks)
            sum += lm.X;

        _handX = sum / landmarks.Count;
    }

    private void Update()
    {
        float targetAngle = Mathf.Lerp(-maxAngle, maxAngle, _handX);
        _currentAngle = Mathf.Lerp(_currentAngle, targetAngle, Time.deltaTime * smoothSpeed);
        _rectTransform.rotation = Quaternion.Euler(0f, 0f, _currentAngle);
    }
}