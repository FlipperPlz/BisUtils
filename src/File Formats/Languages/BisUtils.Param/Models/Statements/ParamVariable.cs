namespace BisUtils.Param.Models.Statements;

using Core.IO;
using Enumerations;
using FResults;
using Literals;
using Options;
using Stubs;
using Stubs.Holders;

public interface IParamVariable : IParamStatement
{
    string VariableName { get; }
    ParamOperatorType VariableOperator { get; }
    IParamLiteral? VariableValue { get; }

    byte GetStatementId();
}

public class ParamVariable : ParamStatement, IParamVariable
{
    public string? VariableName { get; set; }
    public ParamOperatorType VariableOperator { get; set; }
    public IParamLiteral? VariableValue { get; set; }
    public override byte StatementId => GetStatementId();

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
            throw new Exception(); //TODO: ERROR
        }
    }



    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        if (GetStatementId() is 5)
        {
            writer.Write((byte) VariableOperator);
        }
        writer.WriteAsciiZ(VariableName!, options);
        return VariableValue?.Binarize(writer, options) ?? Result.Fail($"Failed to write value of variable {VariableName}");
    }

    public sealed override Result Debinarize(BisBinaryReader reader, ParamOptions options)
    {
        VariableOperator = ParamOperatorType.Assign;
        if (options.LastStatementId == 5)
        {
            VariableOperator = (ParamOperatorType) reader.ReadByte();
        }

        reader.ReadAsciiZ(out var name, options);
        VariableName = name;
        //TODO Read Variable AGHGGH C# GENERICS FUCK OFF PLEASE
        return Result.Fail("TODODODODODODODODO");
    }

    public override Result Validate(ParamOptions options)
    {
        if (VariableName is not null)
        {
            return Result.Fail("No Variable Name!");
        }

        if (VariableOperator is not ParamOperatorType.Assign && VariableValue is not ParamArray)
        {
            return Result.Fail("Invalid Operator!");
        }

        return VariableValue is null ? Result.Fail("No Variable Value!") : Result.Ok();
    }

    public override Result ToParam(out string str, ParamOptions options) => throw new NotImplementedException();

}
