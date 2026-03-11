
using System.Collections.Generic;

namespace TrackerPro.Unity
{
  public class NormalizedRectListAnnotationController : AnnotationController<RectangleListAnnotation>
  {
    private IList<NormalizedRect> _currentTarget;

    public void DrawNow(IList<NormalizedRect> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawLater(IList<NormalizedRect> target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget);
    }
  }
}
