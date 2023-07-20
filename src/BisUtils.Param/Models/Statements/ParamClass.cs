namespace BisUtils.Param.Models.Statements;

using System.Text;
using Core.Extensions;
using Core.IO;
using Errors;
using Extensions;
using Factories;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Options;
using Stubs;
using Stubs.Holders;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IParamClass : IParamExternalClass, IParamStatementHolder
{
    string? InheritedClassname { get; set;  }
    Result LocateParamParent(out IParamExternalClass? clazz);
}

public class ParamClass : ParamStatement, IParamClass
{

    public override byte StatementId => 0;
    public string ClassName { get; set; } = "";
    public string? InheritedClassname { get; set; }
    public List<IParamStatement> Statements { get; set; }

    public ParamClass(
        IParamFile file,
        IParamStatementHolder parent,
        string className,
        string? inheritedClassname = null,
        List<IParamStatement>? statements = null
    ) : base(file, parent)
    {
        Statements = statements ?? new List<IParamStatement>();
        ClassName = className;
        InheritedClassname = inheritedClassname;
    }

    public ParamClass(IParamFile file, IParamStatementHolder parent, BisBinaryReader reader, ParamOptions options) :
        base(file, parent, reader, options)
    {
        Statements = new List<IParamStatement>();
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Validate(ParamOptions options)
    {
        if (options.MissingParentIsError && !(LastResult = LocateParamParent(out _)))
        {
            return LastResult;
        }

        if (options.DuplicateClassnameIsError && ParentClass.HasClass(ClassName, out var duplicateClass))
        {
            return Result.Fail(new ParamDuplicateClassnameError(this, duplicateClass!));
        }


        return Result.Ok();
    }


    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.WriteAsciiZ(ClassName, options.Charset);
        writer.Write(writer.BaseStream.Position);
        return LastResult = Result.ImmutableOk();
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
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
            value.WithReasons(ParamStatementFactory.ReadStatement(ParamFile, this, reader, options, out var statement)
                .Reasons);
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
        if (InheritedClassname is not (null or ""))
        {
            return this.HasClass(InheritedClassname, out clazz)
                ? Result.Ok()
                : Result.Fail(new ParamStatementNotFoundError(InheritedClassname, this));
        }

        clazz = null;
        return LastResult = Result.ImmutableOk();
    }



    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append("class ").Append(ClassName);
        if (InheritedClassname is { } super)
        {
            builder.Append(" : ").Append(super);
        }

        builder.Append(" {\n");

        foreach (var statement in Statements)
        {
            statement.WriteParam(ref builder, options);
            builder.Append('\n');
        }
        builder.Append("\n};");

        return LastResult = Result.Ok();
    }
}
