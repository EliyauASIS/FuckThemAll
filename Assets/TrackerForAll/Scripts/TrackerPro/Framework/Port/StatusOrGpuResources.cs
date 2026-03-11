
using System;

namespace TrackerPro
{
  public class StatusOrGpuResources : StatusOr<GpuResources>
  {
    public StatusOrGpuResources(IntPtr ptr) : base(ptr) { }

    protected override void DeleteMpPtr()
    {
      UnsafeNativeMethods.mp_StatusOrGpuResources__delete(ptr);
    }

    private Status _status;
    public override Status status
    {
      get
      {
        if (_status == null || _status.isDisposed)
        {
          UnsafeNativeMethods.mp_StatusOrGpuResources__status(mpPtr, out var statusPtr).Assert();

          GC.KeepAlive(this);
          _status = new Status(statusPtr);
        }
        return _status;
      }
    }

    public override bool Ok()
    {
      return SafeNativeMethods.mp_StatusOrGpuResources__ok(mpPtr);
    }

    public override GpuResources Value()
    {
      UnsafeNativeMethods.mp_StatusOrGpuResources__value(mpPtr, out var gpuResourcesPtr).Assert();
      Dispose();

      return new GpuResources(gpuResourcesPtr);
    }
  }
}
