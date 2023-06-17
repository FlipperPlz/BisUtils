﻿namespace BisUtils.Core.Parsing;

using System.Text;
using Error;
using Extensions;

public class BisStringStepper
{
    protected string Content;

    public BisStringStepper(string content) => Content = content;
    public int Position { get; private set; } = -1;
    public char? CurrentChar { get; private set; } = null;
    public char? PreviousChar { get; private set; } = null;

    public bool HasNext() => PeekForward() is not null;

    public bool IsEOF() => CurrentChar is null && Position > Content.Length;

    public char? MoveForward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return JumpTo(Position + count);
    }

    public char? MoveBackward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return JumpTo(Position - count);
    }

    public char? PeekForward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return Content.GetOrNull(Position + count);
    }

    public string ReadChars(int count, bool includeFirst = false)
    {
        var i = 0;
        var builder = new StringBuilder();
        if (includeFirst && count >= 1)
        {
            i++;
            builder.Append(CurrentChar);
        }

        while (i != count)
        {
            i++;
            builder.Append(MoveForward());
        }

        return builder.ToString();
    }

    public string GetWhile(Func<BisStringStepper, bool> condition)
    {
        var builder = new StringBuilder();
        while (condition(this))
        {
            builder.Append(CurrentChar);
        }

        return builder.ToString();
    }

    public char? PeekAt(int position) => Content.GetOrNull(position);

    public void ResetLexer(string? content = null)
    {
        if (content is not null)
        {
            Content = content;
        }

        Position = -1;
        PreviousChar = null;
        CurrentChar = null;
    }

    public string PeekForwardMulti(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        var endPosition = Position + count;

        if (endPosition > Content.Length)
        {
            endPosition = Content.Length;
        }

        return Content.Substring(Position, endPosition - Position);
    }

    public char? PeekBackWard(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return Content.GetOrNull(Position - count);
    }

    public string PeekBackwardMulti(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        var startPosition = Position - count;

        if (startPosition < 0)
        {
            startPosition = 0;
        }

        return Content.Substring(startPosition, Position - startPosition);
    }

    public char? JumpTo(int position)
    {
        Position = position;
        PreviousChar = Content.GetOrNull(position - 1);
        return CurrentChar = Content.GetOrNull(position);
    }

    public override string ToString() => Content;
}