
using System;

namespace TrackerPro
{
  public class FaceGeometryPacket : Packet<FaceGeometry.FaceGeometry>
  {
    /// <summary>
    ///   Creates an empty <see cref="FaceGeometryPacket" /> instance.
    /// </summary>
    public FaceGeometryPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public FaceGeometryPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public FaceGeometryPacket At(Timestamp timestamp)
    {
      return At<FaceGeometryPacket>(timestamp);
    }

    public override FaceGeometry.FaceGeometry Get()
    {
      UnsafeNativeMethods.mp_Packet__GetFaceGeometry(mpPtr, out var serializedProto).Assert();
      GC.KeepAlive(this);

      var geometry = serializedProto.Deserialize(FaceGeometry.FaceGeometry.Parser);
      serializedProto.Dispose();

      return geometry;
    }

    public override StatusOr<FaceGeometry.FaceGeometry> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
