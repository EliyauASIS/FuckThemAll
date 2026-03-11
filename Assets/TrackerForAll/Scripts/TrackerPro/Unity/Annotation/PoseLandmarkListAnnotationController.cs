
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity
{
  public class PoseLandmarkListAnnotationController : AnnotationController<PoseLandmarkListAnnotation>
  {
    [SerializeField] private bool _visualizeZ = false;

    public IList<NormalizedLandmark> _currentTarget;

    public void DrawNow(IList<NormalizedLandmark> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawNow(NormalizedLandmarkList target)
    {
            
            DrawNow(target?.Landmark);
    }

    public void DrawLater(IList<NormalizedLandmark> target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    public void DrawLater(NormalizedLandmarkList target)
    {
      DrawLater(target?.Landmark);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget, _visualizeZ);
    }
  }
}
