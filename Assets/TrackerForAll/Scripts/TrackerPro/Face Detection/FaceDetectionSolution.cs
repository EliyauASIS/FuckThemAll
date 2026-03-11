

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity.FaceDetection
{
  public class FaceDetectionSolution : ImageSourceSolution<FaceDetectionGraph>
  {
    [SerializeField] public DetectionListAnnotationController faceDetectionsAnnotationController;

    public FaceDetectionGraph.ModelType modelType
    {
      get => graphRunner.modelType;
      set => graphRunner.modelType = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    protected override void OnStartRun()
    {
      if (!runningMode.IsSynchronous())
      {
        graphRunner.OnFaceDetectionsOutput += OnFaceDetectionsOutput;
      }

      SetupAnnotationController(faceDetectionsAnnotationController, ImageSourceProvider.ImageSource);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      List<Detection> faceDetections = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out faceDetections, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() => graphRunner.TryGetNext(out faceDetections, false));
      }

      faceDetectionsAnnotationController.DrawNow(faceDetections);
            foreach (var trackingEventHandler in trackingEventHandlers)
            {
                trackingEventHandler.OnPoseUpdateEvent.Invoke(this);
            }
        }

    private void OnFaceDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
      faceDetectionsAnnotationController.DrawLater(eventArgs.value);
    }
  }
}
