using System.Text;

namespace BisUtils.Core.IO;

public class BisBinaryReader : BinaryReader
{
    public BisBinaryReader(Stream input) : base(input)
    {
    }

    public BisBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
    {
    }

    public BisBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen)
    {
    }
}