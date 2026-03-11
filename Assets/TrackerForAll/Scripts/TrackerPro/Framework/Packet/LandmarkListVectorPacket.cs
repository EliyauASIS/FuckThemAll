
using System;
using System.Collections.Generic;

namespace TrackerPro
{
  public class LandmarkListVectorPacket : Packet<List<LandmarkList>>
  {
    /// <summary>
    ///   Creates an empty <see cref="LandmarkListVectorPacket" /> instance.
    /// </summary>
    public LandmarkListVectorPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public LandmarkListVectorPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public LandmarkListVectorPacket At(Timestamp timestamp)
    {
      return At<LandmarkListVectorPacket>(timestamp);
    }

    public override List<LandmarkList> Get()
    {
      UnsafeNativeMethods.mp_Packet__GetLandmarkListVector(mpPtr, out var serializedProtoVector).Assert();
      GC.KeepAlive(this);

      var landmarkLists = serializedProtoVector.Deserialize(LandmarkList.Parser);
      serializedProtoVector.Dispose();

      return landmarkLists;
    }

    public override StatusOr<List<LandmarkList>> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
