namespace BisUtils.Param.Models.Statements;

using System.Text;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Factories;
using FResults;
using FResults.Extensions;
using Literals;
using Microsoft.Extensions.Logging;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamVariable : IParamStatement, IParamLiteralHolder
{
    string VariableName { get; set; }
    ParamOperatorType VariableOperator { get; set; }
    IParamLiteral VariableValue { get; set; }
}

public abstract class ParamVariableBase : ParamStatement, IParamVariable
{

    public abstract string VariableName { get; set; }
    public abstract ParamOperatorType VariableOperator { get; set; }
    public abstract IParamLiteral VariableValue { get; set; }
    public IParamLiteralHolder? ParentHolder { get => null; set => throw new NotSupportedException(); }

    public List<IParamLiteral> Literals
    {
        get => new List<IParamLiteral>() { VariableValue };
        set => VariableValue = value[0];
    }

    protected ParamVariableBase(IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(file, parent, logger)
    {
    }

    protected ParamVariableBase(BisBinaryReader reader, ParamOptions options, IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
    }

    public Result WriteLiterals(ref StringBuilder builder, ParamOptions options) =>
        VariableValue.WriteParam(ref builder, options);
}

public class ParamVariable : ParamVariableBase, IParamVariable
{
    public sealed override string VariableName { get; set; } = null!;
    public sealed override ParamOperatorType VariableOperator { get; set; }
    public override byte StatementId => GetStatementId();
    public sealed override IParamLiteral VariableValue { get; set; } = null!;


    public byte GetStatementId()
    {
        if (VariableOperator != ParamOperatorType.Assign)
        {
            return 5;
        }

        return VariableValue is IParamArray ? (byte)2 : (byte)1;
    }


    public ParamVariable(string variableName, IParamLiteral variableValue, ParamOperatorType operatorType, IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(file, parent, logger)
    {
        VariableName = variableName;
        VariableOperator = operatorType;
        VariableValue = variableValue;
    }

    public ParamVariable(BisBinaryReader reader, ParamOptions options, IParamFile file, IParamStatementHolder parent, ILogger? logger) : base(reader, options, file, parent, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (GetStatementId() is 5)
        {
            writer.Write((byte) VariableOperator);
        }
        writer.WriteAsciiZ(VariableName, options);
        return VariableValue.Binarize(writer, options);
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        VariableOperator = ParamOperatorType.Assign;
        if (options.LastStatementId == 5)
        {
            VariableOperator = (ParamOperatorType) reader.ReadByte();
        }

        var result = reader.ReadAsciiZ(out var name, options);
        VariableName = name;
        result.WithReasons(ParamLiteralFactory.ReadLiteral(reader, options, out var value, ParamFile, this, Logger).Reasons);
        VariableValue = value!;
        return result;
    }

    public override Result Validate(ParamOptions options)
    {
        if (VariableOperator is not ParamOperatorType.Assign && VariableValue is not ParamArray)
        {
            return Result.Fail("Invalid Operator!");
        }

        return Result.Ok();
    }

    public override Result WriteParam(ref StringBuilder builder, ParamOptions options)
    {
        builder.Append(VariableName);
        if (VariableValue is IParamArray)
        {
            builder.Append("[]");
        }

        switch (VariableOperator)
        {
            case ParamOperatorType.Assign:
                builder.Append('=');
                break;
            case ParamOperatorType.AddAssign:
                builder.Append("+=");
                break;
            case ParamOperatorType.SubAssign:
                builder.Append("-=");
                break;
        }

        var result = VariableValue.WriteParam(ref builder, options);
        return result;
    }
}
