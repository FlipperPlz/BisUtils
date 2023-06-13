namespace BisUtils.Core.Binarize.Exceptions;

public class DebinarizeFailedException : IOException
{
    private string _message;

    public DebinarizeFailedException(IEnumerable<string> errors) => 
        _message = string.Join("\n", errors);


    public override string Message => "There";
}