
using System;

namespace TrackerPro
{
  public class ClassificationListPacket : Packet<ClassificationList>
  {
    /// <summary>
    ///   Creates an empty <see cref="ClassificationListPacket" /> instance.
    /// </summary>
    public ClassificationListPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public ClassificationListPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public ClassificationListPacket At(Timestamp timestamp)
    {
      return At<ClassificationListPacket>(timestamp);
    }

    public override ClassificationList Get()
    {
      UnsafeNativeMethods.mp_Packet__GetClassificationList(mpPtr, out var serializedProto).Assert();
      GC.KeepAlive(this);

      var classificationList = serializedProto.Deserialize(ClassificationList.Parser);
      serializedProto.Dispose();

      return classificationList;
    }

    public override StatusOr<ClassificationList> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
