
using System;

namespace TrackerPro
{
  public class StatusOrGpuBuffer : StatusOr<GpuBuffer>
  {
    public StatusOrGpuBuffer(IntPtr ptr) : base(ptr) { }

    protected override void DeleteMpPtr()
    {
      UnsafeNativeMethods.mp_StatusOrGpuBuffer__delete(ptr);
    }

    private Status _status;
    public override Status status
    {
      get
      {
        if (_status == null || _status.isDisposed)
        {
          UnsafeNativeMethods.mp_StatusOrGpuBuffer__status(mpPtr, out var statusPtr).Assert();

          GC.KeepAlive(this);
          _status = new Status(statusPtr);
        }
        return _status;
      }
    }

    public override bool Ok()
    {
      return SafeNativeMethods.mp_StatusOrGpuBuffer__ok(mpPtr);
    }

    public override GpuBuffer Value()
    {
      UnsafeNativeMethods.mp_StatusOrGpuBuffer__value(mpPtr, out var gpuBufferPtr).Assert();
      Dispose();

      return new GpuBuffer(gpuBufferPtr);
    }
  }
}
