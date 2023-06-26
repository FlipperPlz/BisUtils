namespace BisUtils.Core.IO;

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
