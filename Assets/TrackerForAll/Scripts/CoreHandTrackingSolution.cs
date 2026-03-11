

using System.Collections;
using System.Collections.Generic;
using TrackerPro.Unity;
using TrackerPro.Unity.HandTracking;
using UnityEngine;

namespace TrackerPro
{
    public class CoreHandTrackingSolution : ImageSourceSolution<HandTrackingGraph>
    {
        public List<Detection> palmDetections = null;
        public List<NormalizedRect> handRectsFromPalmDetections = null;
        public List<NormalizedLandmarkList> handLandmarks = null;
        public List<LandmarkList> handWorldLandmarks = null;
        public List<NormalizedRect> handRectsFromLandmarks = null;
        public List<ClassificationList> handedness = null;
        public HandTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public int maxNumHands
        {
            get => graphRunner.maxNumHands;
            set => graphRunner.maxNumHands = value;
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

        protected override void OnStartRun()
        {

            if (!runningMode.IsSynchronous())
            {
                   
            }

            var imageSource = ImageSourceProvider.ImageSource;
           
        }

        protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
        {
            graphRunner.AddTextureFrameToInputStream(textureFrame);
        }

        protected override IEnumerator WaitForNextValue()
        {

            if (runningMode == RunningMode.Sync)
            {
                var _ = graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness, true);
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() => graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness, false));
            }


            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }

        }

        
    }
}
