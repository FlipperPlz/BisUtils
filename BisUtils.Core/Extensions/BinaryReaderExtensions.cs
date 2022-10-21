
// ReSharper disable once CheckNamespace

using System.Text;

// ReSharper disable once CheckNamespace
namespace System.IO 
{
    public static class BinaryReaderExtensions 
    {
        public static int ReadCompactInteger(this BinaryReader reader) 
        {
            var value = 0;
            for (var i = 0;; ++i) 
            {
                var v = reader.ReadByte();
                value |= v & 0x7F << (7 * i);
                if((v & 0x80) == 0) 
                    break;
            }

            return value;
        }
        
        public static string ReadAsciiZ(this BinaryReader reader) 
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != '\0') bytes.Add(b);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}

