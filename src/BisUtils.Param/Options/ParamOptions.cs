namespace BisUtils.Param.Options;

using System.Text;
using Core.Binarize.Options;
using Core.Binarize.Utils;
using Core.Options;
using Models.Literals;
using Models.Statements;
using Models.Stubs;
using Utils;

public class ParamOptions : IBinarizationOptions, IAsciizLimiterOptions
{
    public int AsciiLengthTimeout { get; set; } = 1024;
    public Encoding Charset { get; set; } = Encoding.UTF8;
    public Endianness ByteOrder { get; set; } = Endianness.Little;
    public bool IgnoreValidation { get; set; }
    public ParamLiteralIdFoster LiteralIdFoster { get; set; } = DefaultLiteralFoster;
    public ParamStatementIdFoster StatementIdFoster { get; set; } = DefaultStatementFoster;

    private static byte DefaultStatementFoster(IParamStatement statement) => statement switch
    {
        ParamClass => 0,
        IParamVariableBase variable => variable.GetStatementId(),
        ParamExternalClass => 3,
        ParamDelete => 4,
        _ => throw new NotSupportedException()
    };

    private static byte DefaultLiteralFoster(IParamLiteralBase literal) => literal switch
    {
        ParamString => 0,
        ParamFloat => 1,
        ParamInt => 2,
        ParamArray => 3,
        _ => throw new NotSupportedException()
    };


}
