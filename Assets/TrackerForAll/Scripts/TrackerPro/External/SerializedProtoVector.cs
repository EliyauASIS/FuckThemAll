
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using pb = Google.Protobuf;

namespace TrackerPro
{
  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct SerializedProtoVector
  {
    private readonly IntPtr _data;
    private readonly int _size;

    public void Dispose()
    {
      UnsafeNativeMethods.mp_api_SerializedProtoArray__delete(_data, _size);
    }

    public List<T> Deserialize<T>(pb::MessageParser<T> parser) where T : pb::IMessage<T>
    {
      var protos = new List<T>(_size);

      unsafe
      {
        var protoPtr = (SerializedProto*)_data;

        for (var i = 0; i < _size; i++)
        {
          var serializedProto = Marshal.PtrToStructure<SerializedProto>((IntPtr)protoPtr++);
          protos.Add(serializedProto.Deserialize(parser));
        }
      }

      return protos;
    }
  }
}
