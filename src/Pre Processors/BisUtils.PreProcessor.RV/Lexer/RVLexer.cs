namespace BisUtils.PreProcessor.RV.Lexer;

using System.Collections.Immutable;
using System.Text;
using Core.Parsing;
using Core.Parsing.Errors;
using FResults;

public class RVLexer : BisMutableStringStepper
{
    public static readonly char[] Whitespaces = { ' ', '\t', '\u000B', '\u000C' };

    public RVLexer(string content) : base(content)
    {

    }

    public Result TraverseWhitespace(out int charCount, bool allowEOF = false, bool allowEOL = true,
        bool allowDirectiveEOL = true, bool includeComments = true)
    {
        charCount = 0;
        while (true)
        {
            if (IsEOF())
            {
                if (allowEOF)
                {
                    break;
                }

                return BisEndOfFileError.Instance;
            }

            switch (CurrentChar)
            {
                case null: goto CurrentIsNull;
                case '\\':
                {
                    if (!allowDirectiveEOL)
                    {
                        break;
                    }

                    var next = PeekForward();
                    if (next != '\r' && next != '\n')
                    {
                        break;
                    }

                    charCount++;
                    break;
                }
                case '\r':
                {
                    if (!allowEOL)
                    {
                        return BisEndOfLineError.Instance;
                    }

                    charCount += MoveForward() == '\n' ? 2 : 1;
                    break;
                }
                case '\n':
                {
                    if (!allowEOL)
                    {
                        return BisEndOfLineError.Instance;
                    }

                    charCount++;
                    MoveForward();
                    break;
                }
                default:
                {
                    if (CurrentChar is { } current)
                    {
                        if (!Whitespaces.Contains(current))
                        {
                            break;
                        }

                        MoveForward();
                        charCount++;
                        if (includeComments)
                        {
                            var result = TraverseComment(out var commentLength, out _, allowEOF);
                            charCount += commentLength;
                            if (commentLength == 0)
                            {
                                return result;
                            }
                        }

                        continue;
                    }

                    goto CurrentIsNull;
                }
            }

            CurrentIsNull:
            {
                if (!allowEOF)
                {
                    return BisEndOfFileError.Instance;
                }

                break;
            }
        }

        return Result.ImmutableOk();
    }

    public Result TraverseLine(out int charCount, bool allowEOF = false, bool countDirectiveEOL = false)
    {
        charCount = 0;
        while (true)
        {
            switch (CurrentChar)
            {
                case null:
                    return allowEOF ? Result.ImmutableOk() : BisEndOfFileError.Instance;
                case '\n':
                    charCount++;
                    MoveForward();
                    return Result.ImmutableOk();
                default:
                    charCount++;
                    MoveForward();
                    break;
            }
        }
    }

    public Result TraverseComment(out int charCount, out string? commentText, bool allowEOF = false)
    {
        var instructionStart = Position;
        charCount = 0;
        if (CurrentChar != '/')
        {
            goto NoComment;
        }

        charCount++;
        switch (MoveForward())
        {
            case '/':
            {
                var result = TraverseLine(out var lineLength, allowEOF, true);
                charCount += lineLength;
                RemoveRange(instructionStart..(instructionStart + lineLength), out commentText);
                JumpTo(instructionStart);
                return result;
            }
            case '*':
            {
                var length = instructionStart;
                while (!(PreviousChar == '*' && CurrentChar == '/'))
                {
                    if (IsEOF())
                    {
                        if (allowEOF)
                        {
                            break;
                        }

                        commentText = null;
                        return BisEndOfFileError.Instance;
                    }

                    length++;
                    charCount++;
                    MoveForward();
                }

                RemoveRange(instructionStart..length, out commentText);
                JumpTo(instructionStart);
                return Result.ImmutableOk();
            }

            default:
            {
                MoveBackward();
                charCount--;
                goto NoComment;
            }

        }

        NoComment:
        {
            commentText = null;
            return Result.ImmutableOk();
        }
    }

    public string ReadWord()
    {
        var builder = new StringBuilder();
        while (MoveForward() != null)
        {
            if (CurrentChar is not { } currentChar || Whitespaces.Contains(currentChar))
            {
                break;
            }

            builder.Append(CurrentChar);
        }

        return builder.ToString();
    }


    private static bool IsMacroChar(char? c) => c is { } notNullC && (char.IsLetter(notNullC) || notNullC == '_');

    public string ReadMacroId(bool throwOnNone = false)
    {
        if (CurrentChar is { } currentChar && (char.IsLetter(currentChar) || currentChar == '_'))
        {
            var builder = new StringBuilder();
            builder.Append(CurrentChar);
            while (IsMacroChar(MoveForward()))
            {
                builder.Append(CurrentChar);
            }

            return builder.ToString();
        }

        if (throwOnNone)
        {
            throw new IOException();
        }

        return "";
    }

    public IEnumerable<string> ReadMacroArguments()
    {
        if(CurrentChar != '(')
        {
            return ImmutableArray<string>.Empty;
        }

        var builder = new StringBuilder();
        var args = new List<string>();

        while (true)
        {
            switch (CurrentChar)
            {
                case ',':
                {
                    if (builder.Length >= 1)
                    {
                        args.Add(builder.ToString());
                        builder.Clear();
                    }

                    break;
                }
                case ')': break;
                default:
                {
                    builder.Append(CurrentChar);
                    break;
                }
            }
            if(CurrentChar == ')')
            {
                break;
            }

            MoveForward();
        }

        return args;
    }
}
