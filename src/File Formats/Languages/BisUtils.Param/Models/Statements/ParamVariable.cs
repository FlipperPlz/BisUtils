namespace BisUtils.Param.Models.Statements;

using System.Text;
using Core.Extensions;
using Core.IO;
using Enumerations;
using Factories;
using FResults;
using FResults.Extensions;
using Literals;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamVariable : IParamStatement, IParamLiteralHolder
{
    string VariableName { get; }
    ParamOperatorType VariableOperator { get; }
    IParamLiteral? VariableValue { get; }
}

public class ParamVariable : ParamStatement, IParamVariable
{
    public List<IParamLiteral> Literals => new List<IParamLiteral>();
    public string VariableName { get; set; } = null!;
    public ParamOperatorType VariableOperator { get; set; }
    public override byte StatementId => GetStatementId();
    public IParamLiteral? VariableValue
    {
        get => Literals.GetOrNull(0);
        set
        {
            if (value is { } notnull)
            {
                Literals[0] = notnull;
            }
        }
    }


    public IParamLiteralHolder? ParentHolder => null;

    public byte GetStatementId()
    {
        if (VariableOperator != ParamOperatorType.Assign)
        {
            return 5;
        }

        return VariableValue is IParamArray ? (byte)2 : (byte)1;
    }


    public ParamVariable(IParamFile? file, IParamStatementHolder? parent, string variableName, IParamLiteral? variableValue, ParamOperatorType operatorType = ParamOperatorType.Assign) : base(file, parent)
    {
        VariableName = variableName;
        VariableOperator = operatorType;
        VariableValue = variableValue;
    }

    public ParamVariable(IParamFile? file, IParamStatementHolder? parent, BisBinaryReader reader, ParamOptions options) : base(file, parent, reader, options)
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
        return VariableValue?.Binarize(writer, options) ?? Result.Fail($"Failed to write value of variable {VariableName}");
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
        result.WithReasons(ParamLiteralFactory.ReadLiteral(ParamFile, this, reader, options, out var value).Reasons);
        VariableValue = value;
        return result;
    }

    public override Result Validate(ParamOptions options)
    {
        if (VariableOperator is not ParamOperatorType.Assign && VariableValue is not ParamArray)
        {
            return Result.Fail("Invalid Operator!");
        }

        return VariableValue is null ? Result.Fail("No Variable Value!") : Result.Ok();
    }

    public override Result ToParam(out string str, ParamOptions options)
    {
        var builder = new StringBuilder(VariableName);
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

        var result = VariableValue?.WriteParam(builder, options) ??
                     Result.Fail($"Couldn't write value of {VariableName}.");
        str = builder.ToString();
        return result;

    }

}
