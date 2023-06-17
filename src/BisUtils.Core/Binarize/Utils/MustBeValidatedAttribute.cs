namespace BisUtils.Core.Binarize.Utils;

public class MustBeValidatedAttribute : Attribute
{
    public MustBeValidatedAttribute(string errorMessage) => ErrorMessage = errorMessage;
    public string ErrorMessage { get; }
}
