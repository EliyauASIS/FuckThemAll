
using System;

namespace TrackerPro
{
  public class StatusOrImageFrame : StatusOr<ImageFrame>
  {
    public StatusOrImageFrame(IntPtr ptr) : base(ptr) { }

    protected override void DeleteMpPtr()
    {
      UnsafeNativeMethods.mp_StatusOrImageFrame__delete(ptr);
    }

    private Status _status;
    public override Status status
    {
      get
      {
        if (_status == null || _status.isDisposed)
        {
          UnsafeNativeMethods.mp_StatusOrImageFrame__status(mpPtr, out var statusPtr).Assert();

          GC.KeepAlive(this);
          _status = new Status(statusPtr);
        }
        return _status;
      }
    }

    public override bool Ok()
    {
      return SafeNativeMethods.mp_StatusOrImageFrame__ok(mpPtr);
    }

    public override ImageFrame Value()
    {
      UnsafeNativeMethods.mp_StatusOrImageFrame__value(mpPtr, out var imageFramePtr).Assert();
      Dispose();

      return new ImageFrame(imageFramePtr);
    }
  }
}
