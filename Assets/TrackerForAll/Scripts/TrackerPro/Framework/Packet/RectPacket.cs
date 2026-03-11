
using System;

namespace TrackerPro
{
  public class RectPacket : Packet<Rect>
  {
    /// <summary>
    ///   Creates an empty <see cref="RectPacket" /> instance.
    /// </summary>
    public RectPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public RectPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public RectPacket At(Timestamp timestamp)
    {
      return At<RectPacket>(timestamp);
    }

    public override Rect Get()
    {
      UnsafeNativeMethods.mp_Packet__GetRect(mpPtr, out var serializedProto).Assert();
      GC.KeepAlive(this);

      var rect = serializedProto.Deserialize(Rect.Parser);
      serializedProto.Dispose();

      return rect;
    }

    public override StatusOr<Rect> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
