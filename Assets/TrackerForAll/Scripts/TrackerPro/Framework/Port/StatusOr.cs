
using System;

namespace TrackerPro
{
  public abstract class StatusOr<T> : MpResourceHandle
  {
    public StatusOr(IntPtr ptr) : base(ptr) { }

    public abstract Status status { get; }
    public virtual bool Ok()
    {
      return status.Ok();
    }

    public virtual T ValueOr(T defaultValue = default)
    {
      return Ok() ? Value() : defaultValue;
    }

    /// <exception cref="MediaPipePluginException">Thrown when status is not ok</exception>
    public abstract T Value();
  }
}
