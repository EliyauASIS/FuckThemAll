
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

namespace TrackerPro.Unity.PoseTracking
{
    public enum Body : int
    {
        NOSE = 0,
        LEFT_EYE_INNER = 1,
        LEFT_EYE = 2,
        LEFT_EYE_OUTER = 3,
        RIGHT_EYE_INNER = 4,
        RIGHT_EYE = 5,
        RIGHT_EYE_OUTER = 6,
        LEFT_EAR = 7,
        RIGHT_EAR = 8,
        MOUTH_LEFT = 9,
        MOUTH_RIGHT = 10,
        LEFT_SHOULDER = 11,
        RIGHT_SHOULDER = 12,
        LEFT_ELBOW = 13,
        RIGHT_ELBOW = 14,
        LEFT_WRIST = 15,
        RIGHT_WRIST = 16,
        LEFT_PINKY = 17,
        RIGHT_PINKY = 18,
        LEFT_INDEX = 19,
        RIGHT_INDEX = 20,
        LEFT_THUMB = 21,
        RIGHT_THUMB = 22,
        LEFT_HIP = 23,
        RIGHT_HIP = 24,
        LEFT_KNEE = 25,
        RIGHT_KNEE = 26,
        LEFT_ANKLE = 27,
        RIGHT_ANKLE = 28,
        LEFT_HEEL = 29,
        RIGHT_HEEL = 30,
        LEFT_FOOT_INDEX = 31,
        RIGHT_FOOT_INDEX = 32
    }
    public class PoseTrackingSolution : ImageSourceSolution<PoseTrackingGraph>
    {
        public static PoseTrackingSolution Instance;
        [SerializeField] private RectTransform _worldAnnotationArea;
        [SerializeField] private DetectionAnnotationController _poseDetectionAnnotationController;
        [SerializeField] private MaskAnnotationController _segmentationMaskAnnotationController;
        public PoseLandmarkListAnnotationController poseLandmarksAnnotationController;
        public PoseWorldLandmarkListAnnotationController poseWorldLandmarksAnnotationController;
        public MultiHandLandmarkListAnnotationController multiHandLandmarkListAnnotationController;
        public NormalizedRectAnnotationController roiFromLandmarksAnnotationController;

        private void Awake()
        {
            Instance = this;
        }
        public PoseTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public bool smoothLandmarks
        {
            get => graphRunner.smoothLandmarks;
            set => graphRunner.smoothLandmarks = value;
        }

        public bool enableSegmentation
        {
            get => graphRunner.enableSegmentation;
            set => graphRunner.enableSegmentation = value;
        }

        public bool smoothSegmentation
        {
            get => graphRunner.smoothSegmentation;
            set => graphRunner.smoothSegmentation = value;
        }

        public float minDetectionConfidence
        {
            get => graphRunner.minDetectionConfidence;
            set => graphRunner.minDetectionConfidence = value;
        }

        public float minTrackingConfidence
        {
            get => graphRunner.minTrackingConfidence;
            set => graphRunner.minTrackingConfidence = value;
        }

        protected override void SetupScreen(ImageSource imageSource)
        {
            base.SetupScreen(imageSource);
            _worldAnnotationArea.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();
        }

        protected override void OnStartRun()
        {
            if (!runningMode.IsSynchronous())
            {
                graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
                graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
                graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
                graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
                graphRunner.OnRoiFromLandmarksOutput += OnRoiFromLandmarksOutput;
            }

            var imageSource = ImageSourceProvider.ImageSource;
            SetupAnnotationController(_poseDetectionAnnotationController, imageSource);
            SetupAnnotationController(poseLandmarksAnnotationController, imageSource);
            SetupAnnotationController(poseWorldLandmarksAnnotationController, imageSource);
            SetupAnnotationController(_segmentationMaskAnnotationController, imageSource);
            SetupAnnotationController(multiHandLandmarkListAnnotationController, imageSource);
            _segmentationMaskAnnotationController.InitScreen(imageSource.textureWidth, imageSource.textureHeight);
            SetupAnnotationController(roiFromLandmarksAnnotationController, imageSource);
        }

        protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
        {
            graphRunner.AddTextureFrameToInputStream(textureFrame);
        }

        protected override IEnumerator WaitForNextValue()
        {
            Detection poseDetection = null;
            NormalizedLandmarkList poseLandmarks = null;
            List<NormalizedLandmarkList> handLandmarks = null;
            List<LandmarkList> handWorldLandmarks = null;
            LandmarkList poseWorldLandmarks = null;
            ImageFrame segmentationMask = null;
            NormalizedRect roiFromLandmarks = null;
            List<ClassificationList> handedness = null;
            if (runningMode == RunningMode.Sync)
            {
                var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, true);
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() => graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, false));
            }

            _poseDetectionAnnotationController.DrawNow(poseDetection);
            poseLandmarksAnnotationController.DrawNow(poseLandmarks);
            multiHandLandmarkListAnnotationController.DrawNow(handLandmarks, handedness);
            poseWorldLandmarksAnnotationController.DrawNow(poseWorldLandmarks);
            _segmentationMaskAnnotationController.DrawNow(segmentationMask);
            roiFromLandmarksAnnotationController.DrawNow(roiFromLandmarks);
            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }

        }

        private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
        {

            _poseDetectionAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            poseLandmarksAnnotationController.DrawLater(eventArgs.value);

        }

        private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
        {
            poseWorldLandmarksAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
        {
            _segmentationMaskAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnRoiFromLandmarksOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
        {
            roiFromLandmarksAnnotationController.DrawLater(eventArgs.value);
        }
        private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
        {
            multiHandLandmarkListAnnotationController.DrawLater(eventArgs.value);
        }
        private void OnHandednessOutput(object stream, OutputEventArgs<List<ClassificationList>> eventArgs)
        {
            multiHandLandmarkListAnnotationController.DrawLater(eventArgs.value);
        }
    }
}
