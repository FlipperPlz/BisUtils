namespace BisUtils.EnfPack.Models;

using Core.Binarize.Synchronization;
using Core.IO;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackElement
{
    IEsPackFile PackFile { get; }
}

public abstract class EsPackElement : BisSynchronizableElement<EsPackOptions>, IEsPackElement
{
    public IEsPackFile PackFile { get; }

    protected EsPackElement(IEsPackFile file, ILogger? logger) : base(file, logger) =>
        PackFile = file;

    protected EsPackElement(BisBinaryReader reader, EsPackOptions options, IEsPackFile file, ILogger? logger) : base(reader, options, file, logger) =>
        PackFile = file;
}
