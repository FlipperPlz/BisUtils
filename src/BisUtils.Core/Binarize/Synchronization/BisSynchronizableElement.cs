namespace BisUtils.Core.Binarize.Synchronization;

using FResults;
using Implementation;
using IO;
using Options;

/// <summary>
/// Represents an element in a synchronization hierarchy. See <see cref="IBisSynchronizable{TOptions}"/> for
/// more information
/// </summary>
/// <typeparam name="TOptions">Type of the binarization options.</typeparam>
public interface IBisSynchronizableElement<TOptions> : IStrictBinaryObject<TOptions> where TOptions : IBinarizationOptions
{
    /// <summary>
    /// Event that fires when changes are made to this element.
    /// </summary>
    public event EventHandler? ChangesMade;

    /// <summary>
    /// Event that fires when changes to this elements are saved.
    /// </summary>
    public event EventHandler? ChangesSaved;

    /// <summary>
    /// The root of the synchronization hierarchy this element belongs to.
    /// </summary>
    public IBisSynchronizable<TOptions> SynchronizationRoot { get; }

    /// <summary>
    /// Indicates whether or not this element is stale.
    /// </summary>
    public bool IsStale { get; }
}

public abstract class BisSynchronizableElement<TOptions> : BinaryObject<TOptions>, IBisSynchronizableElement<TOptions> where TOptions : IBinarizationOptions
{
    public bool IsStale { get; private set; }
    public event EventHandler? ChangesMade;
    public event EventHandler? ChangesSaved;

    protected BisSynchronizableElement(IBisSynchronizable<TOptions> synchronizationRoot)
    {
        IsStale = false;
        SynchronizationRoot = synchronizationRoot;
        SynchronizationRoot.ChangesSaved += OnChangesSaved;
    }

    protected BisSynchronizableElement(BisBinaryReader reader, TOptions options,
        IBisSynchronizable<TOptions> synchronizationRoot) : base(reader, options)
    {
        IsStale = false;
        SynchronizationRoot = synchronizationRoot;
        SynchronizationRoot.ChangesSaved += OnChangesSaved;
        SynchronizationRoot.MonitorElement(this);
    }

    ~BisSynchronizableElement()
    {
        SynchronizationRoot.ChangesSaved -= OnChangesSaved;
        SynchronizationRoot.IgnoreElement(this);
    }


    public abstract Result Validate(TOptions options);

    public IBisSynchronizable<TOptions> SynchronizationRoot { get; }

    protected virtual void OnChangesMade(EventArgs e)
    {
        IsStale = true;
        ChangesMade?.Invoke(this, e);
    }

    protected virtual void OnChangesSaved(object? sender, EventArgs e)
    {
        //Sender will usually be 'this' unless there are child elements
        IsStale = false;
        ChangesSaved?.Invoke(sender, e);
    }

}


