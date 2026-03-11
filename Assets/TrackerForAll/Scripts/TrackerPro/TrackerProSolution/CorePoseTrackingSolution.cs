using System.Collections;
using System.Collections.Generic;
using TrackerPro.Unity.PoseTracking;
using UnityEngine;

namespace TrackerPro.Unity
{
    public class CorePoseTrackingSolution : ImageSourceSolution<PoseTrackingGraph>
    {
        public Detection poseDetection = null;
        [HideInInspector]
        public NormalizedLandmarkList poseLandmarks = null;
        [HideInInspector]
        public List<NormalizedLandmarkList> handLandmarks = null;
        [HideInInspector]
        public List<LandmarkList> handWorldLandmarks = null;
        [HideInInspector]
        public LandmarkList poseWorldLandmarks = null;
        [HideInInspector]
        public ImageFrame segmentationMask = null;
        [HideInInspector]
        public NormalizedRect roiFromLandmarks = null;
        [HideInInspector]
        public List<ClassificationList> handedness = null;
        protected override void SetupScreen(ImageSource imageSource)
        {
            base.SetupScreen(imageSource);
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
        }

        protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
        {
            graphRunner.AddTextureFrameToInputStream(textureFrame);
        }

        protected override IEnumerator WaitForNextValue()
        {
           
            if (runningMode == RunningMode.Sync)
            {
                var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, true);
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() => graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out poseWorldLandmarks, out segmentationMask, out roiFromLandmarks, false));
            }
            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }

        }

        private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
        {

        }

        private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            
        }

        private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
        {
        }

        private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
        {
        }

        private void OnRoiFromLandmarksOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
        {
        }
        private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
        {

        }
        private void OnHandednessOutput(object stream, OutputEventArgs<List<ClassificationList>> eventArgs)
        {

        }
    }
}
