namespace BisUtils.Core.Serialization; 

/// <summary>
/// Represents a base class for objects that can be synchronized with a stream.
/// </summary>
public abstract class BisSynchronizable : IDisposable {
    /// <summary>
    /// Gets or sets the base stream for synchronization.
    /// </summary>
    /// <exception cref="Exception">Thrown when trying to set a read-only stream.</exception>
    protected Stream BaseStream {
        get => _baseStream;
        set {
            _baseStream.Dispose();
            _baseStream = value;
            SynchronizeStream(true);
        }
    }
    private bool _synchronized;
    private bool _disposed;
    private Stream _baseStream;

    /// <summary>
    /// Initializes a new instance of the <see cref="BisSynchronizable"/> class.
    /// </summary>
    /// <param name="stream">The base stream to synchronize with.</param>
    protected BisSynchronizable(Stream stream) {
        _baseStream = stream;
    }

    /// <summary>
    /// Synchronizes the stream with the object's state.
    /// </summary>
    /// <param name="forceSynchronization">Whether to force synchronization, even if the object is already synchronized.</param>
    /// <exception cref="Exception">Thrown when trying to synchronize with a read-only stream.</exception>
    public virtual void SynchronizeStream(bool forceSynchronization = false) {
        if(!forceSynchronization && _synchronized) return;
        if (!_baseStream.CanWrite) throw new Exception("Cannot sync to read-only stream!");
    }

    /// <summary>
    /// Desynchronizes the stream from the object's state.
    /// </summary>
    public virtual void DesynchronizeStream() => _synchronized = false;
    
    /// <summary>
    /// Returns the current synchronization state of the stream/object.
    /// </summary>
    public bool IsSynchronized {
        get => _synchronized;
        protected set => _synchronized = value;
    } 
    
    public virtual void Dispose() {
        if(_disposed) return;
        
        _disposed = true;
        GC.SuppressFinalize(this);
        BaseStream.Dispose();
    }
}