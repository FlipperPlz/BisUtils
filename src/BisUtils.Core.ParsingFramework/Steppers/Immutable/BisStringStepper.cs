namespace BisUtils.Core.ParsingFramework.Steppers.Immutable;

using System.Text;
using Core.Extensions;
using Error;
using Logging;
using Microsoft.Extensions.Logging;
using Misc;

public class BisStringStepper : IBisStringStepper, IBisLoggable
{
    public ILogger? Logger { get; }
    public string Content { get; protected set; }
    public int Position { get; private set; } = -1;
    public char? CurrentChar { get; private set; }
    public char? PreviousChar { get; private set; }

    public BisStringStepper(string content, ILogger? logger = default)
    {
        Content = content;
        Logger = logger;
    }

    public BisStringStepper(byte[] contents, Encoding encoding, ILogger? logger = default)
    : this(encoding.GetString(contents), logger)
    {
    }

    public BisStringStepper
    (
        BinaryReader reader,
        Encoding encoding,
        int? length = null,
        long? stringStart = null,
        StepperDisposalOption readDisposalOption = StepperDisposalOption.JumpBackToStart,
        ILogger? logger = default
    )
    {
        Logger = logger;
        var backupStart = reader.BaseStream.Position;
        var start = backupStart;
        if (stringStart is {  } st)
        {
            start = st;
            reader.BaseStream.Seek(st, SeekOrigin.Begin);
        }
        var byteCount = length ?? (int)(reader.BaseStream.Length - reader.BaseStream.Position);

        Content = encoding.GetString(reader.ReadBytes(byteCount));
        switch (readDisposalOption)
        {
            case StepperDisposalOption.JumpBackToStart:
                reader.BaseStream.Seek(backupStart, SeekOrigin.Begin);
                break;
            case StepperDisposalOption.JumpToStringStart:
                reader.BaseStream.Seek(start, SeekOrigin.Begin);
                break;
            case StepperDisposalOption.Dispose:
                reader.Dispose();
                break;
            case StepperDisposalOption.JumpToStringEnd:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(readDisposalOption), readDisposalOption, null);
        }
    }

    public virtual char? MoveForward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return JumpTo(Position + count);
    }

    public virtual char? MoveBackward(int count = 1)
    {
        ExceptionHelpers.ThrowArgumentNotPositiveException(count);
        return JumpTo(Position - count);
    }

    public char? PeekAt(int position) => Content.GetOrNull(position);

    public string GetRange(Range range) => Content[range];

    public virtual char? JumpTo(int position)
    {
        Position = position;
        PreviousChar = Content.GetOrNull(position - 1);
        return CurrentChar = Content.GetOrNull(position);
    }

    public virtual void ResetStepper(string? content = null)
    {
        if (content is not null)
        {
            Content = content;
        }

        Position = -1;
        PreviousChar = null;
        CurrentChar = null;
    }

}
