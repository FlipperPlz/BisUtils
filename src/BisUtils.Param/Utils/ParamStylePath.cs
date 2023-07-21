namespace BisUtils.Param.Utils;

using Core.IO;
using Enumerations;
using Models;
using Models.Literals;
using Models.Stubs.Holders;
using Options;

public class ParamStylePath : ParamFilePathString
{
    public const string StyleExtension = "style";
    public string DefaultExtension { get; } = "style";
    public string[] AcceptableExtensions { get; } = new[] { "style" };

    public ParamStylePath(IParamString pString) : base(pString)
    {
    }

    public ParamStylePath()
    {

    }
}
