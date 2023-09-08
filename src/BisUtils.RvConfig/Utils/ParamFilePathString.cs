namespace BisUtils.RvConfig.Utils;

using Core.IO;
using Enumerations;
using Models;
using Models.Literals;
using Models.Stubs.Holders;
using Options;

public class ParamFilePathString : ParamPathString
{
    public string CurrentExtension
    {
        get => Path.GetExtension(Value);
        set => base.Value = Path.ChangeExtension(Value, value);
    }

    public ParamFilePathString(IParamString paramString) : base(paramString)
    {
    }

    public ParamFilePathString()
    {
    }
}
