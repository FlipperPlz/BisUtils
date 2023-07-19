namespace BisUtils.Core.Binarize.Synchronization;

using FResults;
using Implementation;
using IO;
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


    public void MonitorElement(BisSynchronizableElement<TOptions> element);
    public void IgnoreElement(BisSynchronizableElement<TOptions> element);

}

public abstract class BisSynchronizable<TOptions> : StrictBinaryObject<TOptions>, IBisSynchronizable<TOptions> where TOptions : IBinarizationOptions
{
    public event EventHandler? ChangesMade;
    public event EventHandler? ChangesSaved;

    public IBisSynchronizable<TOptions> SynchronizationRoot { get; } = null!;

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

    public bool IsStale { get; private set; }

    protected BisSynchronizable(BisBinaryReader reader, TOptions options, Stream? syncTo) : base(reader, options)
    {
        SynchronizationRoot = this;
        SynchronizationStream = syncTo;
    }

    protected BisSynchronizable()
    {

    }

    protected BisSynchronizable(Stream? syncTo)
    {
        SynchronizationRoot = this;
        ChangesMade += OnChangesMade;
        SynchronizationStream = syncTo;
    }


    public Result SynchronizeWithStream(TOptions options)
    {
        LastResult = Result.Ok();
        if (SynchronizationStream is { } stream)
        {
            var writer = new BisBinaryWriter(stream, options.Charset, false);
            LastResult = Binarize(writer, options);
        }

        OnChangesSaved(EventArgs.Empty);
        return LastResult;
    }

    public void MonitorElement(BisSynchronizableElement<TOptions> element) => element.ChangesMade += OnChangesMade;

    public void IgnoreElement(BisSynchronizableElement<TOptions> element) => element.ChangesMade -= OnChangesMade;

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

}

