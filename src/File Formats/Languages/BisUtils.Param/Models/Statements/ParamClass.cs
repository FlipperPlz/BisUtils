namespace BisUtils.Param.Models.Statements;

using System.Text;
using Core.Family;
using Core.IO;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Stubs;
using Stubs.Holders;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IParamClass : IParamExternalClass, IParamStatementHolder
{
    string? InheritedClassname { get; }
    Result LocateParamParent(out IParamExternalClass? clazz);

    string IParamStatementHolder.WriteStatements(ParamOptions options)
    {
        TryWriteStatements(out var str, options);
        return str;
    }

    Result IParamStatementHolder.TryWriteStatements(StringBuilder builder, ParamOptions options)
    {
        var result = ToParam(out var str, options);
        builder.Append(str);
        return result;
    }

    StringBuilder IParamStatementHolder.WriteStatements(out Result result, ParamOptions options)
    {
        var builder = new StringBuilder();
        result = WriteParam(builder, options);
        return builder;
    }

    Result IParamStatementHolder.TryWriteStatements(out string str, ParamOptions options)
    {
        str = string.Join('\n', Statements.Select(s => s.ToParam(options)));
        return Result.Ok();
    }
}

public class ParamClass : ParamStatementHolder, IParamClass
{
    public byte StatementId => 0;
    public string ClassName { get; set; }
    public string? InheritedClassname { get; set; }
    public IFamilyParent? Parent { get; set; }

    public ParamClass(
        IParamFile? file,
        IParamStatementHolder? parent,
        string className,
        string? inheritedClassname = null,
        IEnumerable<IParamStatement>? statements = null
    ) : base(file, parent, statements)
    {
        ClassName = className;
        InheritedClassname = inheritedClassname;
    }

    public ParamClass(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            throw new Exception(); //TODO: ERROR
        }
    }

    public override Result Validate(ParamOptions options) => throw new NotImplementedException();


    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(ClassName, options.Charset);
        writer.Write(writer.BaseStream.Position);
        return LastResult = Result.ImmutableOk();
    }

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        var value = reader.ReadAsciiZ(out var className, options);
        ClassName = className;
        var classBodyOffset = reader.ReadInt32();
        var classHeaderEnd = reader.BaseStream.Position;
        reader.BaseStream.Seek(classBodyOffset, SeekOrigin.Begin);
        value.WithReasons(reader.ReadAsciiZ(out var super, options).Reasons);
        if (value.IsFailed)
        {
            return value;
        }
        InheritedClassname = super;


        for (var i = 0; i < reader.ReadCompactInteger(); i++)
        {
            value.WithReasons(ParamStatement.DebinarizeStatement(ParamFile, this, reader, options, out var statement).Reasons);
            if (statement is null)
            {
                return value.WithReason(new Error()
                {
                    Message = $"Statement number {i} in class '{ClassName}' returned null"
                });
            }

            if (value.IsFailed)
            {
                return value;
            }
            Statements.Add(statement);
        }

        reader.BaseStream.Seek(classHeaderEnd, SeekOrigin.Begin);
        return value;
    }

    public Result LocateParamParent(out IParamExternalClass? clazz)
    {
        if (InheritedClassname == string.Empty)
        {
            clazz = null;
            return LastResult = Result.ImmutableOk();
        }
        //TODO

        clazz = null;
        return LastResult = Result.Fail($"Could not locate parent class of {ClassName}");
    }
}
