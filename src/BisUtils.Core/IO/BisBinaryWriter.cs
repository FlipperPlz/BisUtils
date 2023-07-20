namespace BisUtils.Core.IO;

using System.Buffers;
using System.Data;
using System.Text;
using Binarize.Options;

public class BisBinaryWriter : BinaryWriter
{
    public BisBinaryWriter(Stream output) : base(output)
    {
    }

    public BisBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
    {
    }

    public BisBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
    {
    }

    public void WriteAsciiZ(string str, Encoding encoding)
    {
        var bytes = encoding.GetBytes(str);
        Write(bytes, 0, bytes.Length);
        Write((byte)0); // Null-terminate the string
    }

    public void Write(Stream stream, bool leaveOpen = false)
    {
        if (!stream.CanRead)
        {
            throw new ReadOnlyException("Stream must be readable");
        }

        stream.Position = 0;

        var buffer = ArrayPool<byte>.Shared.Rent(8192);

        try
        {
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, 8192)) > 0)
            {
                BaseStream.Write(buffer, 0, bytesRead);
            }
        }
        finally
        {
            if (!leaveOpen)
            {
                stream.Close();
            }
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public async Task WriteAsync(Stream stream, bool leaveOpen = false, CancellationToken token = default)
    {
        if (!stream.CanRead)
        {
            throw new ReadOnlyException("Stream must be readable");
        }

        stream.Position = 0;

        var buffer = ArrayPool<byte>.Shared.Rent(8192);

        try
        {
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, token)) > 0)
            {
                await BaseStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
            }
        }
        finally
        {
            if (!leaveOpen)
            {
                stream.Close();
            }
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public void WriteAsciiZ(string str, IBinarizationOptions options) => WriteAsciiZ(str, options.Charset);

    public void WriteCompactInteger(int data)
    {
        do
        {
            var current = data % 0x80;
            // ReSharper disable once PossibleLossOfFraction
            data = (int) Math.Floor((decimal) (data / 0x80000000));

            if (data is not char.MinValue)
            {
                current |= 0x80;
            }

            Write((byte) current);
        } while (data > 0x7F);

        if (data is not char.MinValue)
        {
            Write((byte) data);
        }
    }
}
