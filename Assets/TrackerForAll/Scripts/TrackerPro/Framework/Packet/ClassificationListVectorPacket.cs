
using System;
using System.Collections.Generic;

namespace TrackerPro
{
  public class ClassificationListVectorPacket : Packet<List<ClassificationList>>
  {
    /// <summary>
    ///   Creates an empty <see cref="ClassificationListVectorPacket" /> instance.
    /// </summary>
    public ClassificationListVectorPacket() : base(true) { }

    [UnityEngine.Scripting.Preserve]
    public ClassificationListVectorPacket(IntPtr ptr, bool isOwner = true) : base(ptr, isOwner) { }

    public ClassificationListVectorPacket At(Timestamp timestamp)
    {
      return At<ClassificationListVectorPacket>(timestamp);
    }

    public override List<ClassificationList> Get()
    {
      UnsafeNativeMethods.mp_Packet__GetClassificationListVector(mpPtr, out var serializedProtoVector).Assert();
      GC.KeepAlive(this);

      var classificationLists = serializedProtoVector.Deserialize(ClassificationList.Parser);
      serializedProtoVector.Dispose();

      return classificationLists;
    }

    public override StatusOr<List<ClassificationList>> Consume()
    {
      throw new NotSupportedException();
    }
  }
}
