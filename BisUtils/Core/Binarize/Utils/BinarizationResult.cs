namespace BisUtils.Core.Binarize.Utils;

public readonly struct BinarizationResult
{

    public BinarizationResult(string message) { Errors = new[] { message }; }
    
    public static readonly BinarizationResult Okay = new BinarizationResult { Errors = Enumerable.Empty<string>() };
    public static readonly BinarizationResult Invalid = new BinarizationResult { Errors = new []{ "There was an error validating the binary structure." } };
    
    
    public IEnumerable<string> Errors { get; init; }
    
    public bool IsValid => !Errors.Any();
}