namespace BisUtils.Core.Binarize.Utils;

public readonly struct BinarizationResult
{

    public BinarizationResult(string message) { Errors = new[] { message }; }
    
    public static readonly BinarizationResult Okay = new BinarizationResult { Errors = Enumerable.Empty<string>() };
    
    public IEnumerable<string> Errors { get; init; }
    
    public bool IsValid => !Errors.Any();
}