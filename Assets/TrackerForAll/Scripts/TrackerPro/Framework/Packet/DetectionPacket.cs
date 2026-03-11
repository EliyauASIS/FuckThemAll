
using System;

namespace TrackerPro
{
  public class DetectionPacket : Packet<Detection>
  {
    /// <summary>
    ///   Creates an empty <see cref="DetectionPacket" /> instance.
    /// </summary>
    public DetectionPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public DetectionPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public DetectionPacket At(Timestamp timestamp)
    {
      return At<DetectionPacket>(timestamp);
    }

    public override Detection Get()
    {
      UnsafeNativeMethods.mp_Packet__GetDetection(mpPtr, out var serializedProto).Assert();
      GC.KeepAlive(this);

      var detection = serializedProto.Deserialize(Detection.Parser);
      serializedProto.Dispose();

      return detection;
    }

    public override StatusOr<Detection> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
