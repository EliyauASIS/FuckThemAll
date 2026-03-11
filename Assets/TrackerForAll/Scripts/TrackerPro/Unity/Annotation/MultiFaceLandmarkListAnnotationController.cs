
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity
{
  public class MultiFaceLandmarkListAnnotationController : AnnotationController<MultiFaceLandmarkListAnnotation>
  {
    [SerializeField] private bool _visualizeZ = false;
        [HideInInspector]
    public IList<NormalizedLandmarkList> currentTarget;

    public void DrawNow(IList<NormalizedLandmarkList> target)
    {
      currentTarget = target;
      SyncNow();
    }

    public void DrawLater(IList<NormalizedLandmarkList> target)
    {
      UpdateCurrentTarget(target, ref currentTarget);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(currentTarget, _visualizeZ);
    }
  }
}
