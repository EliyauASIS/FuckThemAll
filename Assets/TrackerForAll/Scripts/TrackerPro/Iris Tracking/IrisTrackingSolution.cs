
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity.IrisTracking
{
  public class IrisTrackingSolution : ImageSourceSolution<IrisTrackingGraph>
  {
    [SerializeField] public DetectionListAnnotationController _faceDetectionsAnnotationController;
    [SerializeField] public NormalizedRectAnnotationController _faceRectAnnotationController;
    [SerializeField] public FaceLandmarkListAnnotationController _faceLandmarksWithIrisAnnotationController;

    protected override void OnStartRun()
    {
      if (!runningMode.IsSynchronous())
      {
        graphRunner.OnFaceDetectionsOutput += OnFaceDetectionsOutput;
        graphRunner.OnFaceRectOutput += OnFaceRectOutput;
        graphRunner.OnFaceLandmarksWithIrisOutput += OnFaceLandmarksWithIrisOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      SetupAnnotationController(_faceDetectionsAnnotationController, imageSource);
      SetupAnnotationController(_faceRectAnnotationController, imageSource);
      SetupAnnotationController(_faceLandmarksWithIrisAnnotationController, imageSource);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      List<Detection> faceDetections = null;
      NormalizedRect faceRect = null;
      NormalizedLandmarkList faceLandmarksWithIris = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out faceDetections, out faceRect, out faceLandmarksWithIris, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() => graphRunner.TryGetNext(out faceDetections, out faceRect, out faceLandmarksWithIris, false));
      }

      _faceDetectionsAnnotationController.DrawNow(faceDetections);
      _faceRectAnnotationController.DrawNow(faceRect);
      _faceLandmarksWithIrisAnnotationController.DrawNow(faceLandmarksWithIris);
            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }
        }

    private void OnFaceDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
      _faceDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceRectOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
    {
      _faceRectAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnFaceLandmarksWithIrisOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
    {
      _faceLandmarksWithIrisAnnotationController.DrawLater(eventArgs.value);
    }

  }
}
