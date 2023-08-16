namespace BisUtils.RvConfig.Utils;

using Core.IO;
using Enumerations;
using Models;
using Models.Literals;
using Models.Stubs.Holders;
using Options;

public class ParamPathString
{
    public IParamString ParamString { get; set; }
    public string Value
    {
        get => ParamString.Value;
        set => ParamString.Value = RVPathUtilities.NormalizePboPath(value);
    }


    public ParamPathString()
    {

    }

    public ParamPathString(IParamString paramString) => ParamString = paramString;
}
