
using System;
using System.Collections.Generic;

namespace TrackerPro
{
  public class NormalizedRectVectorPacket : Packet<List<NormalizedRect>>
  {
    /// <summary>
    ///   Creates an empty <see cref="NormalizedRectVectorPacket" /> instance.
    /// </summary>
    public NormalizedRectVectorPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public NormalizedRectVectorPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public NormalizedRectVectorPacket At(Timestamp timestamp)
    {
      return At<NormalizedRectVectorPacket>(timestamp);
    }

    public override List<NormalizedRect> Get()
    {
      UnsafeNativeMethods.mp_Packet__GetNormalizedRectVector(mpPtr, out var serializedProtoVector).Assert();
      GC.KeepAlive(this);

      var normalizedRects = serializedProtoVector.Deserialize(NormalizedRect.Parser);
      serializedProtoVector.Dispose();

      return normalizedRects;
    }

    public override StatusOr<List<NormalizedRect>> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
