using System.Collections;
using System.Collections.Generic;
using TrackerPro;
using TrackerPro.Unity;
using TrackerPro.Unity.FaceMesh;
using UnityEngine;
namespace TrackerPro
{
    public class CoreFaceMeshSolution : ImageSourceSolution<FaceMeshGraph>
    {
        public List<Detection> faceDetections = null;
        public List<NormalizedLandmarkList> multiFaceLandmarks = null;
        public List<NormalizedRect> faceRectsFromLandmarks = null;
        public List<NormalizedRect> faceRectsFromDetections = null;
        public int maxNumFaces
        {
            get => graphRunner.maxNumFaces;
            set => graphRunner.maxNumFaces = value;
        }

        public bool refineLandmarks
        {
            get => graphRunner.refineLandmarks;
            set => graphRunner.refineLandmarks = value;
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
                graphRunner.OnFaceDetectionsOutput += OnFaceDetectionsOutput;
                graphRunner.OnMultiFaceLandmarksOutput += OnMultiFaceLandmarksOutput;
                graphRunner.OnFaceRectsFromLandmarksOutput += OnFaceRectsFromLandmarksOutput;
                graphRunner.OnFaceRectsFromDetectionsOutput += OnFaceRectsFromDetectionsOutput;
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
                var _ = graphRunner.TryGetNext(out faceDetections, out multiFaceLandmarks, out faceRectsFromLandmarks, out faceRectsFromDetections, true);
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() => graphRunner.TryGetNext(out faceDetections, out multiFaceLandmarks, out faceRectsFromLandmarks, out faceRectsFromDetections, false));
            }


            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }
        }

        private void OnFaceDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
        {
        }

        private void OnMultiFaceLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
        {
        }

        private void OnFaceRectsFromLandmarksOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
        {
        }

        private void OnFaceRectsFromDetectionsOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
        {
        }

    }
}