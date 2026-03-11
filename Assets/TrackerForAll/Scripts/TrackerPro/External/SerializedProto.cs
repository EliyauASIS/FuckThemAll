
using System;
using System.Runtime.InteropServices;

using pb = Google.Protobuf;

namespace TrackerPro
{
  [StructLayout(LayoutKind.Sequential)]
  internal readonly struct SerializedProto
  {
    private readonly IntPtr _str;
    private readonly int _length;

    public void Dispose()
    {
      UnsafeNativeMethods.delete_array__PKc(_str);
    }

    public T Deserialize<T>(pb::MessageParser<T> parser) where T : pb::IMessage<T>
    {
      var bytes = new byte[_length];
      Marshal.Copy(_str, bytes, 0, bytes.Length);
      return parser.ParseFrom(bytes);
    }
  }
}
