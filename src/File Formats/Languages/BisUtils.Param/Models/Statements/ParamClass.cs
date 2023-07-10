namespace BisUtils.Param.Models.Statements;

using Core.Extensions;
using Core.IO;
using Enumerations;
using Errors;
using Factories;
using FResults;
using FResults.Extensions;
using FResults.Reasoning;
using Literals;
using Options;
using Stubs;
using Stubs.Holders;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IParamClass : IParamExternalClass, IParamStatementHolder
{
    string? InheritedClassname { get; }
    Result LocateParamParent(out IParamExternalClass? clazz);
}

public class ParamClass : ParamStatementHolder, IParamClass
{

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

    public ParamClass(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) :
        base(file, parent, reader, options)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public byte StatementId => 0;

    public string ClassName { get; set; } = "";
    public string? InheritedClassname { get; set; }

    public override Result Validate(ParamOptions options)
    {
        if (options.MissingParentIsError && !(LastResult = LocateParamParent(out _)))
        {
            return LastResult;
        }

        if (options.DuplicateClassnameIsError && ParentClass?.HasClass(ClassName, out var duplicateClass) == true)
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

    public void SyncToContext(IParamStatementHolder? holder)
    {
        ParentClass = holder;
        ParamFile = holder?.ParamFile;
    }

    public Result LocateParamParent(out IParamExternalClass? clazz)
    {
        if (InheritedClassname is not (null or ""))
        {
            return (this as IParamStatementHolder).HasClass(InheritedClassname, out clazz)
                ? Result.Ok()
                : Result.Fail(new ParamStatementNotFoundError(InheritedClassname, this));
        }

        clazz = null;
        return LastResult = Result.ImmutableOk();
    }

    public IEnumerable<T> GetStatements<T>() =>
        (this as IParamStatementHolder).GetStatements<T>();

    public IEnumerable<IParamExternalClass> LocateAllClasses() =>
        (this as IParamStatementHolder).LocateAllClasses();

    public IEnumerable<IParamExternalClass> LocateAllClasses(string classname) =>
        (this as IParamStatementHolder).LocateAllClasses(classname);

    public IParamExternalClass? LocateAnyClass(string classname) =>
        (this as IParamStatementHolder).LocateAnyClass(classname);

    public IEnumerable<IParamClass> LocateBaseClasses() =>
        (this as IParamStatementHolder).LocateBaseClasses();

    public IEnumerable<IParamClass> LocateBaseClasses(string classname) =>
        (this as IParamStatementHolder).LocateBaseClasses(classname);

    public IParamClass? LocateBaseClass(string classname) =>
        (this as IParamStatementHolder).LocateBaseClass(classname);

    public IEnumerable<ParamExternalClass> LocateExternalClasses() =>
        (this as IParamStatementHolder).LocateExternalClasses();

    public IEnumerable<ParamExternalClass> LocateExternalClasses(string classname) =>
        (this as IParamStatementHolder).LocateExternalClasses(classname);

    public ParamExternalClass? LocateExternalClass(string classname) =>
        (this as IParamStatementHolder).LocateExternalClass(classname);

    public IEnumerable<IParamDelete> LocateDeleteStatements() =>
        (this as IParamStatementHolder).LocateDeleteStatements();

    public IEnumerable<IParamDelete> LocateDeleteStatements(string target) =>
        (this as IParamStatementHolder).LocateDeleteStatements(target);

    public IParamDelete? LocateDeleteStatement(string target) =>
        (this as IParamStatementHolder).LocateDeleteStatement(target);

    public IEnumerable<IParamVariable> LocateAllVariables() =>
        (this as IParamStatementHolder).LocateAllVariables();

    public IEnumerable<IParamVariable> LocateVariables(string name) =>
        (this as IParamStatementHolder).LocateVariables(name);

    public IEnumerable<IParamVariable> LocateVariables<T>(string name) where T : IParamLiteral =>
        (this as IParamStatementHolder).LocateVariables<T>(name);

    public IParamVariable? LocateVariable(string name) =>
        (this as IParamStatementHolder).LocateVariable(name);

    public IParamArray? LocateArray(string name, out ParamOperatorType? op) =>
        (this as IParamStatementHolder).LocateArray(name, out op);

    public IParamVariable? LocateVariable<T>(string name) where T : IParamLiteral =>
        (this as IParamStatementHolder).LocateVariable<T>(name);

    public T? EvaluateVariable<T>(string name) where T : IParamLiteral =>
        (this as IParamStatementHolder).EvaluateVariable<T>(name);

    public bool HasVariable(string name, out IParamVariable? variable) =>
        (this as IParamStatementHolder).HasVariable(name, out variable);

    public bool HasVariable<T>(string name, out IParamVariable? variable) where T : IParamLiteral =>
        (this as IParamStatementHolder).HasVariable<T>(name, out variable);

    public bool HasExternalClass(string name, out ParamExternalClass? clazz) =>
        (this as IParamStatementHolder).HasExternalClass(name, out clazz);

    public bool HasBaseClass(string name, out IParamClass? clazz) =>
        (this as IParamStatementHolder).HasBaseClass(name, out clazz);

    public bool HasClass(string name, out IParamExternalClass? clazz) =>
        (this as IParamStatementHolder).HasClass(name, out clazz);

    public bool WasDeleted(string target, out IParamDelete? deleteStatement) =>
        (this as IParamStatementHolder).WasDeleted(target, out deleteStatement);
}
