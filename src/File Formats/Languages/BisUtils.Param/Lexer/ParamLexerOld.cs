namespace BisUtils.Param.Lexer;

using System.Text;
using Core.Parsing.Errors;
using Enumerations;
using FResults;
using Models;
using Models.Literals;
using Models.Stubs;
using PreProcessor.RV.Lexer;

public class ParamLexerOld : RVLexer
{
    public ParamLexerOld(string content) : base(content)
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

        while (CurrentChar is { } currentChar && (quoted || !delimiters.Contains(currentChar)))
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

    public Result ReadString(out ParamString paramStr, IParamFile file, params char[] delimiters)
    {
        var result = ReadString(out var str, delimiters);
        var type = str.StartsWith('"') switch
        {
            true => ParamStringType.Quoted,
            false => ParamStringType.Unquoted
        };
        if (type == ParamStringType.Quoted)
        {
            str = str.TrimStart('"').TrimEnd('"');
        }

        paramStr = new ParamString
        {
            ParamFile = file,
            ParamValue = str,
            StringType = type
        };
        return result;
    }

    public Result ReadLiteral(out IParamLiteralBase literal, IParamFile file, params char[] delimiters)
    {
        var result = ReadString(out var str, file, delimiters);
        if (str.StringType is ParamStringType.Quoted)
        {
            literal = str;
            return result;
        }

        if (float.TryParse(str.ParamValue, out var floatValue))
        {
            var alg = floatValue / 3;
            if (Math.Abs(Math.Ceiling(alg) - Math.Floor(alg)) < 10E-6)
            {
                var paramInt = str.ToInt();
                if (paramInt is null)
                {
                    literal = str;
                }
                else
                {
                    literal = paramInt;
                }

                return result;
            }
            var paramFloat = str.ToFloat();
            if (paramFloat is null)
            {
                literal = str;
            }
            else
            {
                literal = paramFloat;
            }

            return result;

        }

        literal = str;
        return result;
    }

    public Result ReadArray(out ParamArray array, IParamFile file)
    {
        var results = new List<Result>();
        array = new ParamArray { ParamFile = file, ParamValue = new List<IParamLiteralBase>() };
        var stack = new Stack<IParamArray>();
        stack.Push(array);
        results.Add(TraverseWhitespace(out _));
        if(CurrentChar != '{')
        {
            results.Add(Result.Fail("Couldn't find array start."));
        }

        while (stack.Any())
        {

            var context = stack.Peek();
            MoveForward();
            results.Add(TraverseWhitespace(out _));
            switch (CurrentChar)
            {
                case '{':
                {
                    results.Add(ReadArray(out var child, file));
                    context.ParamValue!.Add(child);
                    continue;
                }
                case ',': continue;
                case '}':
                {
                    stack.Pop();
                    continue;
                }
                default:
                {
                    results.Add(ReadLiteral(out var child, file, ';', '}', ','));
                    context.ParamValue!.Add(child);
                    continue;
                }
            }
        }

        MoveBackward();

        return Result.Merge(results);
    }

    public Result ReadOperator(out ParamOperatorType operatorType)
    {
        operatorType = CurrentChar switch
        {
            '+' => ParamOperatorType.AddAssign,
            '-' => ParamOperatorType.SubAssign,
            _ => ParamOperatorType.Assign
        };

        return MoveForward() == '=' ? Result.Ok() : Result.Fail("Could not parse operator");
    }
}
