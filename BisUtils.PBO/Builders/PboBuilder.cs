namespace BisUtils.PBO.Builders; 

public class PboBuilder {
    private readonly PboFile _pboFile;

    public PboBuilder(PboFile _pboFile) {
        
    }

    public PboFile GetPboFile() => _pboFile;
}