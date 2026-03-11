
using System;
using System.Collections.Generic;

namespace TrackerPro
{
  public class DetectionVectorPacket : Packet<List<Detection>>
  {
    /// <summary>
    ///   Creates an empty <see cref="DetectionVectorPacket" /> instance.
    /// </summary>
    public DetectionVectorPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public DetectionVectorPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public DetectionVectorPacket At(Timestamp timestamp)
    {
      return At<DetectionVectorPacket>(timestamp);
    }

    public override List<Detection> Get()
    {
      UnsafeNativeMethods.mp_Packet__GetDetectionVector(mpPtr, out var serializedProtoVector).Assert();
      GC.KeepAlive(this);

      var detections = serializedProtoVector.Deserialize(Detection.Parser);
      serializedProtoVector.Dispose();

      return detections;
    }

    public override StatusOr<List<Detection>> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
