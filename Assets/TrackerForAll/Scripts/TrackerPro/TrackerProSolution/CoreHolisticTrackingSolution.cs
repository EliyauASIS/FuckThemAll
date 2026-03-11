using System.Collections;
using System.Collections.Generic;
using TrackerPro.Unity.Holistic;
using UnityEngine;
namespace TrackerPro.Unity
{
    public class CoreHolisticTrackingSolution : ImageSourceSolution<HolisticTrackingGraph>
    {
        [HideInInspector]
        public Detection poseDetection = null;
        [HideInInspector]
        public NormalizedLandmarkList faceLandmarks = null;
        [HideInInspector]
        public NormalizedLandmarkList poseLandmarks = null;
        [HideInInspector]
        public NormalizedLandmarkList leftHandLandmarks = null;
        [HideInInspector]
        public NormalizedLandmarkList rightHandLandmarks = null;
        [HideInInspector]
        public LandmarkList poseWorldLandmarks = null;
        [HideInInspector]
        public ImageFrame segmentationMask = null;
        [HideInInspector]
        public NormalizedRect poseRoi = null;

        public HolisticTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public bool smoothLandmarks
        {
            get => graphRunner.smoothLandmarks;
            set => graphRunner.smoothLandmarks = value;
        }

        public bool refineFaceLandmarks
        {
            get => graphRunner.refineFaceLandmarks;
            set => graphRunner.refineFaceLandmarks = value;
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
        }

        protected override void OnStartRun()
        {
            if (!runningMode.IsSynchronous())
            {
                graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
                graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
                graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
                graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
                graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
                graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
                graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
                graphRunner.OnPoseRoiOutput += OnPoseRoiOutput;
            }


        }

        protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
        {
            graphRunner.AddTextureFrameToInputStream(textureFrame);
        }

        protected override IEnumerator WaitForNextValue()
        {


            if (runningMode == RunningMode.Sync)
            {
                var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, true);
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() =>
                  graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, false));
            }
            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }
        }

        private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
        {

        }

        private void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {

        }

        private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {

        }

        private void OnLeftHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
        }

        private void OnRightHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
        }

        private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
        {
        }

        private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
        {
        }

        private void OnPoseRoiOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
        {
        }
    }
}