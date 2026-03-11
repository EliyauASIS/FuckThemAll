using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity.HandTracking
{
    public class HandTracker : MonoBehaviour
    {
        [Header("Thresholds")]
        [SerializeField] private float extendedThreshold = 0.04f;
        [SerializeField] private float curledThreshold = 0.30f;
        [SerializeField] private int requiredFrames = 5;

        public event Action OnGestureDetected;
        public event Action OnGestureEnded;

        private const int WRIST      = 0;
        private const int INDEX_MCP  = 5;
        private const int INDEX_TIP  = 8;
        private const int MIDDLE_MCP = 9;
        private const int MIDDLE_TIP = 12;
        private const int RING_MCP   = 13;
        private const int RING_TIP   = 16;
        private const int PINKY_MCP  = 17;
        private const int PINKY_TIP  = 20;

        private HandTrackingGraph _graphRunner;
        private List<NormalizedLandmarkList> _latestLandmarks;
        private int _detectedFrames = 0;
        private bool _gestureActive = false;
        private bool _subscribed = false;

        private void Start()
        {
            StartCoroutine(WaitAndSubscribe());
        }

        private IEnumerator WaitAndSubscribe()
        {
            yield return new WaitUntil(() => GetComponent<HandTrackingGraph>() != null);
            _graphRunner = GetComponent<HandTrackingGraph>();

            float elapsed = 0f;
            while (!_subscribed && elapsed < 10f)
            {
                try
                {
                    _graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
                    _subscribed = true;
                }
                catch { }
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        private void OnDestroy()
        {
            if (_graphRunner != null && _subscribed)
                _graphRunner.OnHandLandmarksOutput -= OnHandLandmarksOutput;
        }

        private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
        {
            _latestLandmarks = eventArgs.value;
        }

        private void Update()
        {
            if (!_subscribed || _latestLandmarks == null || _latestLandmarks.Count == 0)
            {
                ResetDetection();
                return;
            }

            bool gestureDetected = false;

            foreach (var hand in _latestLandmarks)
            {
                if (hand == null || hand.Landmark.Count < 21) continue;
                if (IsMiddleFingerUp(hand)) { gestureDetected = true; break; }
            }

            if (gestureDetected)
            {
                _detectedFrames++;
                if (_detectedFrames >= requiredFrames && !_gestureActive)
                {
                    _gestureActive = true;
                    Debug.Log("[HandTracker] 🖕 זוהתה אצבע אמצעית!");
                    OnGestureDetected?.Invoke();
                }
            }
            else
            {
                if (_gestureActive)
                {
                    _gestureActive = false;
                    OnGestureEnded?.Invoke();
                }
                ResetDetection();
            }
        }

        private bool IsMiddleFingerUp(NormalizedLandmarkList hand)
        {
            var lm = hand.Landmark;

            // אמצעית: פרושה כלפי מעלה
            bool middleExtended   = (lm[MIDDLE_MCP].Y - lm[MIDDLE_TIP].Y) > extendedThreshold;
            bool middlePointingUp = lm[MIDDLE_TIP].Y < lm[WRIST].Y;

            // שאר האצבעות (מלבד אגודל): מקופלות
            bool indexCurled = (lm[INDEX_MCP].Y - lm[INDEX_TIP].Y) < curledThreshold;
            bool ringCurled  = (lm[RING_MCP].Y  - lm[RING_TIP].Y)  < curledThreshold;
            bool pinkyCurled = (lm[PINKY_MCP].Y - lm[PINKY_TIP].Y) < curledThreshold;

            return middleExtended && middlePointingUp && indexCurled && ringCurled && pinkyCurled;
        }

        private void ResetDetection()
        {
            _detectedFrames = 0;
            _gestureActive = false;
        }
    }
}