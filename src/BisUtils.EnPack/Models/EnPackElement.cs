namespace BisUtils.EnPack.Models;

using Core.Binarize.Synchronization;
using Core.IO;
using Microsoft.Extensions.Logging;
using Options;

public interface IEnPackElement
{
    IEnPackFile PackFile { get; }
}

public abstract class EnPackElement : BisSynchronizableElement<EnPackOptions>, IEnPackElement
{
    public IEnPackFile PackFile { get; }

    protected EnPackElement(IEnPackFile file, ILogger? logger) : base(file, logger) =>
        PackFile = file;

    protected EnPackElement(BisBinaryReader reader, EnPackOptions options, IEnPackFile file, ILogger? logger) : base(reader, options, file, logger) =>
        PackFile = file;
}
