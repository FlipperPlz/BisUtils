namespace BisUtils.Core.Render.Color;

using Binarize;
using Extensions;
using FResults;
using IO;
using Options;

public interface IColor : IBinaryObject<IBisColorOptions>
{
    public float Red { get; }
    public float Green { get; }
    public float Blue { get; }
    public float Alpha { get; }

    public int PackColor();
}

public struct Color : IColor
{
    public Result? LastResult { get; private set; }

    public float Red { get; private set; }
    public float Green { get; private set; }
    public float Blue { get; private set; }
    public float Alpha { get; private set; }

    public int PackColor()
    {
        var value = 0;
        value |= (int)Math.Abs(Alpha) << 24;
        value |= (int)Math.Abs(Red) << 16;
        value |= (int)Math.Abs(Green) << 8;  // Shift G8 to the third leftmost byte
        value |= (int)Math.Abs(Blue);
        return value;
    }

    public Color(float red, float green, float blue, float alpha)
    {
        Red = red;
        Green = green;
        Blue = blue;
        Alpha = alpha;
    }

    public Color(BisBinaryReader reader, IBisColorOptions options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public Color(BisBinaryReader reader, bool writePacked)
    {
        if (!Debinarize(reader, writePacked))
        {
            LastResult!.Throw();
        }
    }

    public Result Binarize(BisBinaryWriter writer, IBisColorOptions options)
    {
        switch (options.UsePackedColor)
        {
            case false:
            {
                writer.Write(Red);
                writer.Write(Green);
                writer.Write(Blue);
                writer.Write(Alpha);
                return LastResult = Result.Ok();
            }
            case true:
            {
                writer.Write(PackColor());
                return LastResult = Result.Ok();
            }
        }
    }

    public Result Debinarize(BisBinaryReader reader, bool writePacked) =>
        Debinarize(reader, new BisColorOptions { UsePackedColor = writePacked });

    public Result Debinarize(BisBinaryReader reader, IBisColorOptions options)
    {
        switch (options.UsePackedColor)
        {
            case false:
            {
                Red = reader.ReadSingle();
                Green = reader.ReadSingle();
                Blue = reader.ReadSingle();
                Alpha = reader.ReadSingle();
                return LastResult = Result.Ok();
            }
            case true:
            {
                var value = reader.ReadInt32();
                Alpha = (value >> 24) & 0xff;
                Red = (value >> 16) & 0xff;
                Green = (value >>  8) & 0xff;
                Blue = value & 0xff;
                return LastResult = Result.Ok();
            }
        }
    }
}
