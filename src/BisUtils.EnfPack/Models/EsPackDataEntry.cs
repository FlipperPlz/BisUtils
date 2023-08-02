namespace BisUtils.EnfPack.Models;

using Core.Extensions;
using Core.IO;
using Enumerations;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackDataEntry : IEsPackEntry
{
    public uint Offset { get; set; }
    public uint PackedSize { get; set; }
    public uint OriginalSize { get; set; }
    public uint Crc { get; set; }
    public EsPackCompressionType CompressionType { get; set; }
    public byte CompressionLevel { get; set; }
}


public class EsPackDataEntry : EsPackEntry, IEsPackDataEntry
{
    public uint Offset { get; set; }
    public uint PackedSize { get; set; }
    public uint OriginalSize { get; set; }
    public uint Crc { get; set; }
    public EsPackCompressionType CompressionType { get; set; }
    public byte CompressionLevel { get; set; }

    public EsPackDataEntry(IEsPackFile file, IEsPackDirectory parent, ILogger? logger) : base(file, parent, logger)
    {
    }

    public EsPackDataEntry(BisBinaryReader reader, EsPackOptions options, IEsPackDirectory parent, IEsPackFile synchronizationRoot, ILogger? logger) : base(reader, options, parent, synchronizationRoot, logger, true)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, EsPackOptions options)
    {
        LastResult = base.Binarize(writer, options);
        writer.Write(Offset);
        writer.Write(PackedSize);
        writer.Write(OriginalSize);
        writer.Write(Crc);
        writer.Write((byte)CompressionType);
        writer.Write(CompressionLevel);
        writer.Write(new byte[6]);
        return LastResult;
    }

    public sealed override Result Debinarize(BisBinaryReader reader, EsPackOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        Offset = reader.ReadUInt32();
        PackedSize = reader.ReadUInt32();
        OriginalSize = reader.ReadUInt32();
        Crc = reader.ReadUInt32();
        CompressionType = (EsPackCompressionType)reader.ReadByte(); //Weird enum is more than 1 byte
        CompressionLevel = reader.ReadByte();
        reader.BaseStream.Seek(6, SeekOrigin.Current);
        return LastResult;
    }

    public override Result Validate(EsPackOptions options) => throw new NotImplementedException();

}
