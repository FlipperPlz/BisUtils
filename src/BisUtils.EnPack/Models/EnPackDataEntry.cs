namespace BisUtils.EnPack.Models;

using BisUtils.Core.Extensions;
using BisUtils.Core.IO;
using BisUtils.EnPack.Enumerations;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEnPackDataEntry : IEnPackEntry
{
    public uint Offset { get; set; }
    public uint PackedSize { get; set; }
    public uint OriginalSize { get; set; }
    public uint Crc { get; set; }
    public EnPackCompressionType CompressionType { get; set; }
    public byte CompressionLevel { get; set; }
}


public class EnPackDataEntry : EnPackEntry, IEnPackDataEntry
{
    public uint Offset { get; set; }
    public uint PackedSize { get; set; }
    public uint OriginalSize { get; set; }
    public uint Crc { get; set; }
    public EnPackCompressionType CompressionType { get; set; }
    public byte CompressionLevel { get; set; }

    public EnPackDataEntry(IEnPackFile file, IEnPackDirectory parent, ILogger? logger) : base(file, parent, logger)
    {
    }

    public EnPackDataEntry(BisBinaryReader reader, EnPackOptions options, IEnPackDirectory parent, IEnPackFile synchronizationRoot, ILogger? logger) : base(reader, options, parent, synchronizationRoot, logger, true)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, EnPackOptions options)
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

    public sealed override Result Debinarize(BisBinaryReader reader, EnPackOptions options)
    {
        LastResult = base.Debinarize(reader, options);
        Offset = reader.ReadUInt32();
        PackedSize = reader.ReadUInt32();
        OriginalSize = reader.ReadUInt32();
        Crc = reader.ReadUInt32();
        CompressionType = (EnPackCompressionType)reader.ReadByte(); //Weird enum is more than 1 byte
        CompressionLevel = reader.ReadByte();
        reader.BaseStream.Seek(6, SeekOrigin.Current);
        return LastResult;
    }

    public override Result Validate(EnPackOptions options) => throw new NotImplementedException();

}
