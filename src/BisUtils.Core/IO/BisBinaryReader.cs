namespace BisUtils.Core.IO;

using System.Text;
using Binarize;
using Binarize.Implementation;
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

    public IEnumerable<T> ReadStrictIndexedList<T, TOptions>(TOptions options, int? count = null, Action<T>? afterRead = null)where T : StrictBinaryObject<TOptions>, new() where TOptions : IBinarizationOptions =>
        ReadIndexedList<StrictBinaryObject<TOptions>, T, TOptions>(options, count, afterRead);

    public IEnumerable<T> ReadIndexedList<T, TOptions>(TOptions options, int? count = null, Action<T>? afterRead = null)where T : BinaryObject<TOptions>, new() where TOptions : IBinarizationOptions =>
        ReadIndexedList<BinaryObject<TOptions>, T, TOptions>(options, count, afterRead);

    public IEnumerable<T> ReadIndexedList<TBase, T, TOptions>(int count, TOptions options)
        where TBase : IBinarizable<TOptions>
        where T : TBase, new()
        where TOptions : IBinarizationOptions
    {
        for(var i = 0; i < count; i++)
        {
            yield return (T) Activator.CreateInstance(typeof(T), new object?[] {this, options} , null)!;
        }
    }

    public IEnumerable<T> ReadIndexedList<TBase, T, TOptions>(TOptions options, int? count = null, Action<T>? afterRead = null)
        where TBase : IBinarizable<TOptions>
        where T : TBase, new()
        where TOptions : IBinarizationOptions
    {
        var c = count ??= ReadInt32();
        var list = new List<T>(c);
        var fireAfter = afterRead is not null;
        for(var i = 0; i < c; i++)
        {
            var lastIn = (T)Activator.CreateInstance(typeof(T), new object?[] { this, options }, null)!;
            list.Add(lastIn);
            if(fireAfter)
            {
                afterRead!(lastIn);
            }
        }

        return list;
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
