namespace BisUtils.EnfPack.Models;

using Core.Binarize.Synchronization;
using Core.IO;
using Microsoft.Extensions.Logging;
using Options;

public interface IEsPackElement
{
    IEsPackFile PackFile { get; }
}

public abstract class EsPackElement : BisSynchronizableElement<EsPackOptions>
{
    protected EsPackElement(IEsPackFile synchronizationRoot, ILogger? logger) : base(synchronizationRoot, logger)
    {
    }

    protected EsPackElement(BisBinaryReader reader, EsPackOptions options, IEsPackFile synchronizationRoot, ILogger? logger) : base(reader, options, synchronizationRoot, logger)
    {
    }

}
