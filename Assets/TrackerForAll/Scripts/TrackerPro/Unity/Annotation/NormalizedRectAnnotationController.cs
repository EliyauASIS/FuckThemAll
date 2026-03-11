
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TrackerPro.Unity
{
  public class NormalizedRectAnnotationController : AnnotationController<RectangleAnnotation>
  {
    public NormalizedRect _currentTarget;

    public void DrawNow(NormalizedRect target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawLater(NormalizedRect target)
    {
     
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    protected override void SyncNow()
    {
      isStale = false;
     if (_currentTarget != null) annotation.width = _currentTarget.Height;
      annotation.Draw(_currentTarget);
    }
  }
}
