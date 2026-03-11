
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity.HandTracking
{
  public class HandTrackingSolution : ImageSourceSolution<HandTrackingGraph>
  {
    [SerializeField] public DetectionListAnnotationController palmDetectionsAnnotationController;
    [SerializeField] public NormalizedRectListAnnotationController handRectsFromPalmDetectionsAnnotationController;
    [SerializeField] public MultiHandLandmarkListAnnotationController handLandmarksAnnotationController;
    [SerializeField] public NormalizedRectListAnnotationController handRectsFromLandmarksAnnotationController;

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
        graphRunner.OnPalmDetectectionsOutput += OnPalmDetectionsOutput;
        graphRunner.OnHandRectsFromPalmDetectionsOutput += OnHandRectsFromPalmDetectionsOutput;
        graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
        // TODO: render HandWorldLandmarks annotations
        graphRunner.OnHandRectsFromLandmarksOutput += OnHandRectsFromLandmarksOutput;
        graphRunner.OnHandednessOutput += OnHandednessOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      SetupAnnotationController(palmDetectionsAnnotationController, imageSource, true);
      SetupAnnotationController(handRectsFromPalmDetectionsAnnotationController, imageSource, true);
      SetupAnnotationController(handLandmarksAnnotationController, imageSource, true);
      SetupAnnotationController(handRectsFromLandmarksAnnotationController, imageSource, true);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      List<Detection> palmDetections = null;
      List<NormalizedRect> handRectsFromPalmDetections = null;
      List<NormalizedLandmarkList> handLandmarks = null;
      List<LandmarkList> handWorldLandmarks = null;
      List<NormalizedRect> handRectsFromLandmarks = null;
      List<ClassificationList> handedness = null;
       if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() => graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness, false));
      }

    
            palmDetectionsAnnotationController.DrawNow(palmDetections);
            handRectsFromPalmDetectionsAnnotationController.DrawNow(handRectsFromPalmDetections);
            handLandmarksAnnotationController.DrawNow(handLandmarks, handedness);
            // TODO: render HandWorldLandmarks annotations
            handRectsFromLandmarksAnnotationController.DrawNow(handRectsFromLandmarks);

            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }

        }

    private void OnPalmDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
      palmDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandRectsFromPalmDetectionsOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      handRectsFromPalmDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
    {
        handLandmarksAnnotationController.DrawLater(eventArgs.value);
           
    }

    private void OnHandRectsFromLandmarksOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      handRectsFromLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandednessOutput(object stream, OutputEventArgs<List<ClassificationList>> eventArgs)
    {
      handLandmarksAnnotationController.DrawLater(eventArgs.value);
    }
  }
}
