namespace BisUtils.Core.Binarize.Synchronization;

using FResults;
using FResults.Extensions;
using Implementation;
using IO;
using Microsoft.Extensions.Logging;
using Options;

/// <summary>
/// Represents the root of an object that can synchronized with a stream by using the implemented
/// Binarize function. This class inherits from StrictBinaryObject meaning it can be validated.
/// For info on serialization/binarization see the parent package 'BisUtils.Core.Binarize'
/// </summary>
/// <typeparam name="TOptions">Type of options used for validation/reading/writing.</typeparam>
public interface IBisSynchronizable<TOptions> : IBisSynchronizableElement<TOptions> where TOptions : IBinarizationOptions
{
    /// <summary>
    /// The current stream being used for synchronization. Since theres some use-cases where writing
    /// isn't needed, this value is nullable; When the stream is null all events will still be fired
    /// and the IsStale property will be updated.
    /// </summary>
    /// <value>The synchronization stream. Null if not set.</value>
    public Stream? SynchronizationStream { get; }

    /// <summary>
    /// Synchronizes the object with SynchronizationStream.
    /// </summary>
    /// <param name="options">The binarization options to use for synchronization.</param>
    /// <returns>A result indicating the success of the binarization process.</returns>
    public Result SynchronizeWithStream(TOptions options);


}

public abstract class BisSynchronizable<TOptions> : StrictBinaryObject<TOptions>, IBisSynchronizable<TOptions> where TOptions : IBinarizationOptions
{
    public event EventHandler? ChangesMade;
    public event EventHandler? ChangesSaved;

    public IBisSynchronizable<TOptions> SynchronizationRoot { get; } = null!;
    public bool IsStale { get; private set; }
    private Stream? synchronizationStream;
    public Stream? SynchronizationStream
    {
        get => synchronizationStream;
        set
        {
            IsStale = true;
            synchronizationStream = value;
        }
    }


    protected BisSynchronizable(BisBinaryReader reader, TOptions options, Stream? syncTo, ILogger logger) : base(reader, options, logger)
    {
        SynchronizationRoot = this;
        SynchronizationStream = syncTo;
    }

    protected BisSynchronizable(ILogger? logger) : base(logger)
    {

    }

    protected BisSynchronizable(Stream? syncTo, ILogger? logger) : base(logger)
    {
        SynchronizationRoot = this;
        ChangesMade += OnChangesMade;
        SynchronizationStream = syncTo;

    }


    public Result SynchronizeWithStream(TOptions options)
    {
        LastResult = Result.Ok();
        if (this != SynchronizationRoot)
        {
            return LastResult.WithError("Synchronization Error", typeof(BisSynchronizable<TOptions>), "Synchronization can only be performed from the root element.");
        }
        if (SynchronizationStream is { } stream)
        {
            var writer = new BisBinaryWriter(stream, options.Charset, true);
            LastResult = Binarize(writer, options);
        }

        OnChangesSaved(EventArgs.Empty);
        return LastResult;
    }


    protected virtual void OnChangesSaved(EventArgs e)
    {
        ChangesSaved?.Invoke(this, e);
        IsStale = false;
    }


    protected virtual void OnChangesMade(object? sender, EventArgs e)
    {
        ChangesMade?.Invoke(sender, e);
        IsStale = true;
    }

    public void MonitorElement(BisSynchronizableElement<TOptions> element) => element.ChangesMade += OnChangesMade;
    public void IgnoreElement(BisSynchronizableElement<TOptions> element) => element.ChangesMade -= OnChangesMade;

}

