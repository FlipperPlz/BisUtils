namespace BisUtils.Core.Binarize.Utils;

public class MustBeValidatedAttribute : Attribute
{
    public string ErrorMessage { get; }

    public MustBeValidatedAttribute(string errorMessage) => ErrorMessage = errorMessage;
}