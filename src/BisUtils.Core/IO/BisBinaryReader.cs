namespace BisUtils.Core.IO;

using System.Text;
using Binarize.Options;
using FResults;
using Options;

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


    public byte[] ReadBuffer(int length) => ReadBytes(length);

    public string ReadAscii(int length, Encoding encoding) => Encoding.UTF8.GetString(ReadBuffer(length));

    public string ReadAscii(int length, IBinarizationOptions options) => ReadAscii(length, options.Charset);


    public Result SkipAsciiZ<TOptions>(TOptions? options)
        where TOptions : IAsciizLimiterOptions, IBinarizationOptions =>
        ReadAsciiZ(out _, options);

    public Result ReadAsciiZ<TOptions>(
        out string read,
        TOptions? options
    ) where TOptions : IAsciizLimiterOptions, IBinarizationOptions
    {
        int maxLength = options?.AsciiLengthTimeout ?? -1, bytesRead = 0;
        var stringBuffer = new StringBuilder();

        while (true)
        {
            if (maxLength >= 0 && bytesRead >= maxLength)
            {
                read = stringBuffer.ToString();
                return Result.Fail($"AsciiZ String exceeded maximum read length of {maxLength}.");
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
        return Result.Ok();
    }

    public int ReadCompactInteger()
    {
        var value = 0;
        for (var i = 0;; ++i)
        {
            var v = ReadByte();
            value |= v & (0x7F << (7 * i));
            if((v & 0x80) == 0)
            {
                break;
            }
        }

        return value;
    }
}
