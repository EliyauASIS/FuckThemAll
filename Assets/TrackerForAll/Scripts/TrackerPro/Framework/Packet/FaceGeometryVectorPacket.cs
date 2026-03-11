
using System;
using System.Collections.Generic;

namespace TrackerPro
{
  public class FaceGeometryVectorPacket : Packet<List<FaceGeometry.FaceGeometry>>
  {
    /// <summary>
    ///   Creates an empty <see cref="FaceGeometryVectorPacket" /> instance.
    /// </summary>
    public FaceGeometryVectorPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public FaceGeometryVectorPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public FaceGeometryVectorPacket At(Timestamp timestamp)
    {
      return At<FaceGeometryVectorPacket>(timestamp);
    }

    public override List<FaceGeometry.FaceGeometry> Get()
    {
      UnsafeNativeMethods.mp_Packet__GetFaceGeometryVector(mpPtr, out var serializedProtoVector).Assert();
      GC.KeepAlive(this);

      var geometries = serializedProtoVector.Deserialize(FaceGeometry.FaceGeometry.Parser);
      serializedProtoVector.Dispose();

      return geometries;
    }

    public override StatusOr<List<FaceGeometry.FaceGeometry>> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
