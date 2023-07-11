namespace BisUtils.P3D.Models.Face;

using BisUtils.P3D.Utils;
using Core.Binarize;using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using Options;

public interface IRVFace : IStrictBinaryObject<Options.RVFaceOptions>
{
    const int MaxDataCount = 4;
    string Texture { get; set; }
    string Material { get; set; }
    IEnumerable<RVDataVertex> Vertices { get; set; }
    public RVFaceFlags Flags { get; set; }
}

public class RVFace : StrictBinaryObject<Options.RVFaceOptions>, IRVFace
{
    public string Texture { get; set; }
    public string Material { get; set; }
    public IEnumerable<RVDataVertex> Vertices { get; set; }
    public RVFaceFlags Flags { get; set; }

    public RVFace(string texture, string material, IEnumerable<RVDataVertex> vertices, RVFaceFlags flags)
    {
        Texture = texture;
        Material = material;
        Vertices = vertices;
        Flags = flags;
    }

    public RVFace(BisBinaryReader reader, RVFaceOptions options) : base(reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    protected RVFace(BisBinaryReader reader, RVFaceOptions options, bool b) : base(reader, options)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, RVFaceOptions options)
    {
        writer.WriteAsciiZ(Texture, options);
        writer.WriteAsciiZ(Material, options);
        writer.Write(Vertices.Count());
        //TODO: VERSION/EXTENDED SPECIFIC WRITE
        foreach (var vertex in Vertices)
        {
            vertex.Binarize(writer, options);
        }

        return LastResult = Result.Ok();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, RVFaceOptions options)
    {
        //TODO: VERSION/EXTENDED SPECIFIC READ

        reader.ReadAsciiZ(out var texture, options);
        Texture = texture;
        reader.ReadAsciiZ(out var material, options);
        Material = material;

        var nVertices = reader.ReadInt32();
        var vertices = new List<RVDataVertex>(nVertices);
        for (var i = 0; i < nVertices; i++)
        {
            vertices.Add(new RVDataVertex(reader, options));
        }

        Vertices = vertices;

        return LastResult = Result.Ok();
    }

    public override Result Validate(RVFaceOptions options) => LastResult = Result.FailIf(Vertices.Count() > 4, "Face has more than 4 vertices");
}


