
using System;

namespace TrackerPro
{
  public class NormalizedLandmarkListPacket : Packet<NormalizedLandmarkList>
  {
    /// <summary>
    ///   Creates an empty <see cref="NormalizedLandmarkListPacket" /> instance.
    /// </summary>
    public NormalizedLandmarkListPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public NormalizedLandmarkListPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public NormalizedLandmarkListPacket At(Timestamp timestamp)
    {
      return At<NormalizedLandmarkListPacket>(timestamp);
    }

    public override NormalizedLandmarkList Get()
    {
      UnsafeNativeMethods.mp_Packet__GetNormalizedLandmarkList(mpPtr, out var serializedProto).Assert();
      GC.KeepAlive(this);

      var normalizedLandmarkList = serializedProto.Deserialize(NormalizedLandmarkList.Parser);
      serializedProto.Dispose();

      return normalizedLandmarkList;
    }

    public override StatusOr<NormalizedLandmarkList> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
