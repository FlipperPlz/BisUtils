namespace BisUtils.Core.IO;

using System.Text;
using Binarize.Options;

public class BisBinaryWriter : BinaryWriter
{

    public void WriteAsciiZ(string str, Encoding encoding)
    {
        var bytes = encoding.GetBytes(str);
        Write(bytes, 0, bytes.Length);
        Write((byte)0); // Null-terminate the string
    }

    public void WriteAsciiZ(string str, IBinarizationOptions options) => WriteAsciiZ(str, options.Charset);
}

