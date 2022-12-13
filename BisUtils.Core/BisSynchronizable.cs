namespace BisUtils.Core; 

public abstract class BisSynchronizable : IDisposable {
    private Stream SynchronizedStream {
        get => _syncedStream;
        set {
            _syncedStream!.Dispose();
            _syncedStream = value;
            SynchronizeStream(true);
        }
    }
    private bool _disposed, _synchronized;
    private Stream _syncedStream;

    protected BisSynchronizable(Stream stream) {
        _syncedStream = stream;
    }

    public virtual void SynchronizeStream(bool forceSynchronization = false) {
        if(!forceSynchronization && _synchronized) return;
        if (!_syncedStream.CanWrite) throw new Exception("Cannot sync to read-only stream!");
        _synchronized = true;
    }

    public virtual void DesynchronizeStream() {
        _synchronized = false;
    } 
    
    public bool IsSynchronized() => _synchronized;
    
    public virtual void Dispose() {
        if(_disposed) return;
        
        _disposed = true;
        GC.SuppressFinalize(this);
        SynchronizedStream.Dispose();
    }
}