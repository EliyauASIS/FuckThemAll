
using System;

namespace TrackerPro
{
  public class NormalizedRectPacket : Packet<NormalizedRect>
  {
    /// <summary>
    ///   Creates an empty <see cref="NormalizedRectPacket" /> instance.
    /// </summary>
    public NormalizedRectPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public NormalizedRectPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public NormalizedRectPacket At(Timestamp timestamp)
    {
      return At<NormalizedRectPacket>(timestamp);
    }

    public override NormalizedRect Get()
    {
      UnsafeNativeMethods.mp_Packet__GetNormalizedRect(mpPtr, out var serializedProto).Assert();
      GC.KeepAlive(this);

      var normalizedRect = serializedProto.Deserialize(NormalizedRect.Parser);
      serializedProto.Dispose();

      return normalizedRect;
    }

    public override StatusOr<NormalizedRect> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
