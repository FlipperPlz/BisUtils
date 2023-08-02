namespace BisUtils.EnfPack.Models;

using Core.Binarize.Synchronization;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackFile : IBisSynchronizable<EsPackOptions>
{

}

public class EsPackFile : BisSynchronizable<EsPackOptions>
{

    public EsPackFile(BisBinaryReader reader, EsPackOptions options, Stream? syncTo, ILogger? logger) : base(reader, options, syncTo, logger)
    {
    }

    public EsPackFile(ILogger? logger) : base(logger)
    {
    }

    public EsPackFile(Stream? syncTo, ILogger? logger) : base(syncTo, logger)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, EsPackOptions options) =>
        throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, EsPackOptions options) =>
        throw new NotImplementedException();

    public override Result Validate(EsPackOptions options) =>
        throw new NotImplementedException();



}
