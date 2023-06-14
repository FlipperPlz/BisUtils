namespace BisUtils.Core.Binarize.Utils;

public class BinarizationResult
{

    public BinarizationResult(string message) => Errors = new List<string> { message };
    public BinarizationResult(List<string> errors) => Errors = errors;

    public static readonly BinarizationResult Okay = new(new List<string>());
    public static readonly BinarizationResult Invalid = new( "There was an error validating the binary structure.");
    public static readonly BinarizationResult AsciiZTimeout = new("There was an error reading a null terminated string (timeout: too long)");


    public List<string> Errors { get; init; }

    public bool IsValid => !Errors.Any();
    public bool IsNotValid => Errors.Any();


    public static BinarizationResult operator +(BinarizationResult left, BinarizationResult right)
    {
        left.Errors.ToList().AddRange(right.Errors);
        return left;
    }
}
