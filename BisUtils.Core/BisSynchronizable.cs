namespace BisUtils.Core; 

public abstract class BisSynchronizable : IDisposable {
    protected Stream BaseStream {
        get => _baseStream;
        set {
            _baseStream!.Dispose();
            _baseStream = value;
            SynchronizeStream(true);
        }
    }
    private bool _disposed, _synchronized;
    private Stream _baseStream;

    protected BisSynchronizable(Stream stream) {
        _baseStream = stream;
    }

    public virtual void SynchronizeStream(bool forceSynchronization = false) {
        if(!forceSynchronization && _synchronized) return;
        if (!_baseStream.CanWrite) throw new Exception("Cannot sync to read-only stream!");
    }

    public virtual void DesynchronizeStream() => _synchronized = false;

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