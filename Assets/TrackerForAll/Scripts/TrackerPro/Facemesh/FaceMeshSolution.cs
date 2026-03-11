

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity.FaceMesh
{
  public class FaceMeshSolution : ImageSourceSolution<FaceMeshGraph>
  {
    [SerializeField] public DetectionListAnnotationController faceDetectionsAnnotationController;
    [SerializeField] public MultiFaceLandmarkListAnnotationController multiFaceLandmarksAnnotationController;
    [SerializeField] public NormalizedRectListAnnotationController faceRectsFromLandmarksAnnotationController;
    [SerializeField] public NormalizedRectListAnnotationController faceRectsFromDetectionsAnnotationController;

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
      SetupAnnotationController(faceDetectionsAnnotationController, imageSource);
      SetupAnnotationController(faceRectsFromLandmarksAnnotationController, imageSource);
      SetupAnnotationController(multiFaceLandmarksAnnotationController, imageSource);
      SetupAnnotationController(faceRectsFromDetectionsAnnotationController, imageSource);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      List<Detection> faceDetections = null;
      List<NormalizedLandmarkList> multiFaceLandmarks = null;
      List<NormalizedRect> faceRectsFromLandmarks = null;
      List<NormalizedRect> faceRectsFromDetections = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out faceDetections, out multiFaceLandmarks, out faceRectsFromLandmarks, out faceRectsFromDetections, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() => graphRunner.TryGetNext(out faceDetections, out multiFaceLandmarks, out faceRectsFromLandmarks, out faceRectsFromDetections, false));
      }

      faceDetectionsAnnotationController.DrawNow(faceDetections);
      multiFaceLandmarksAnnotationController.DrawNow(multiFaceLandmarks);
      faceRectsFromLandmarksAnnotationController.DrawNow(faceRectsFromLandmarks);
      faceRectsFromDetectionsAnnotationController.DrawNow(faceRectsFromDetections);
            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }
        }

    private void OnFaceDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
      faceDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnMultiFaceLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
    {
      multiFaceLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceRectsFromLandmarksOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      faceRectsFromLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceRectsFromDetectionsOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      faceRectsFromDetectionsAnnotationController.DrawLater(eventArgs.value);
    }
  }
}
