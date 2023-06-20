namespace BisUtils.Param.Models.Statements;

using Core.IO;
using Enumerations;
using FResults;
using Literals;
using Options;
using Stubs;

public interface IParamVariableBase : IParamStatement
{
    string VariableName { get; }
    ParamOperatorType VariableOperator { get; }
    IParamLiteralBase? UnboxedVariableValue { get; }

    byte GetStatementId();
}

public interface IParamVariable<out TParamType> : IParamVariableBase where TParamType : IParamLiteralBase
{
    IParamLiteralBase? IParamVariableBase.UnboxedVariableValue => VariableValue;
    TParamType? VariableValue { get; }
}

public class ParamVariable<TParamType> : ParamStatement, IParamVariable<TParamType> where TParamType : IParamLiteralBase
{
    public string VariableName { get; set; }
    public ParamOperatorType VariableOperator { get; set; }

    public byte GetStatementId()
    {
        if (VariableOperator != ParamOperatorType.Assign)
        {
            return 5;
        }

        return VariableValue is IParamArray ? (byte)2 : (byte)1;
    }

    public TParamType? VariableValue { get; set; }

    public ParamVariable(IParamFile? file, IParamStatementHolder? parent, string variableName, TParamType? variableValue) : base(file, parent)
    {
        VariableName = variableName;
        VariableValue = variableValue;
    }

    public ParamVariable(BisBinaryReader reader, ParamOptions options, string variableName, TParamType? variableValue) : base(reader, options)
    {
        VariableName = variableName;
        VariableValue = variableValue;
    }


    public override Result Binarize(BisBinaryWriter writer, ParamOptions options)
    {
        writer.Write(options.StatementIdFoster(this));


        return Result.Ok();
    }

    public override Result Debinarize(BisBinaryReader reader, ParamOptions options) => throw new NotImplementedException();

    public override Result Validate(ParamOptions options) => throw new NotImplementedException();

    public override Result ToParam(out string str, ParamOptions options) => throw new NotImplementedException();

}
