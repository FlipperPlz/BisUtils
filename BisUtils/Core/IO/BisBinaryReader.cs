using System.Text;
using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Options;

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


    public BinarizationResult ReadAsciiZ(out string read, IAsciizLimiterOptions? options)
    {
        int maxLength = options?.AsciiLengthTimeout ?? -1, bytesRead = 0;
        var stringBuffer = new StringBuilder();

        while (true)
        {
            if (maxLength >= 0 && bytesRead >= maxLength)
            {
                read = stringBuffer.ToString();
                return BinarizationResult.AsciiZTimeout;
            }

            var b = ReadByte();
            bytesRead++;

            if (b == 0)
            {
                break;
            }

            stringBuffer.Append((char)b);
        }

        read = stringBuffer.ToString();
        return BinarizationResult.Okay;
    }
}