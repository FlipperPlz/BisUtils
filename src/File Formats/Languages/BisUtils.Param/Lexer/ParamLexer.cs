namespace BisUtils.Param.Lexer;

using System.Text;
using Core.Parsing.Errors;
using FResults;
using PreProcessor.RV.Lexer;

public class ParamLexer : RVLexer
{
    public ParamLexer(string content) : base(content)
    {
    }

    public Result ReadIdentifier(out string keyword, bool allowEOF = false)
    {
        var results = new List<Result>() { TraverseWhitespace(out _) };
        if(CurrentChar is null && !allowEOF)
        {
            keyword = "";
            results.Add(Result.Fail(BisEndOfFileError.Instance));
            return Result.Merge(results);
        }

        var builder = new StringBuilder();
        while (CurrentChar is { } currentChar && (char.IsLetterOrDigit(currentChar) || currentChar == '_'))
        {
            builder.Append(currentChar);
            MoveForward();
        }

        keyword = builder.ToString();
        return Result.Merge(results);
    }

    public Result ReadString(out string stringRead, params char[] delimiters)
    {
        var results = new List<Result>() { TraverseWhitespace(out _) };
        var quoted = false;
        if (CurrentChar == '"')
        {
            quoted = true;
            MoveForward();
        }

        var builder = new StringBuilder();

        while (CurrentChar is { } currentChar && !delimiters.Contains(currentChar))
        {
            if (currentChar is '\n' or '\r')
            {
                if (!quoted)
                {
                    results.Add(TraverseWhitespace(out _));
                    if (currentChar != '#')
                    {
                        // ReSharper disable once RedundantJumpStatement
                        goto Finish;
                    }
                    //TODO PreProcessLine
                    break;
                }
                results.Add(Result.Fail(BisEndOfLineError.Instance));
                goto Finish;
            }

            if (currentChar == '"' && quoted)
            {
                if (MoveForward() != '"')
                {
                    results.Add(TraverseWhitespace(out _));
                    if (currentChar != '\\')
                    {
                        goto Finish;
                    }

                    if (MoveForward() != 'n')
                    {
                        results.Add(Result.Fail("TODO")); //TODO);
                        goto Finish;
                    }
                    results.Add(TraverseWhitespace(out _));
                    if (CurrentChar != '"')
                    {
                        results.Add(Result.Fail("TODO")); //TODO);
                        goto Finish;
                    }

                    builder.Append('\n');
                    MoveForward();
                }
            }

            builder.Append(CurrentChar);
            MoveForward();
        }
        Finish:
        {
            stringRead = builder.ToString();
            return Result.Merge(results);
        }

    }
}
