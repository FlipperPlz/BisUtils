namespace BisUtils.RvConfig.Utils;

using Core.IO;
using Enumerations;
using Models;
using Models.Literals;
using Models.Stubs.Holders;
using Options;

public class ParamMaterialPath : ParamFilePathString
{
    public const string MaterialExtension = "rvmat";
    public string DefaultExtension { get; } = MaterialExtension;
    public string[] AcceptableExtensions { get; } = new[] { MaterialExtension, "bin", "cpp" };

    public ParamMaterialPath(IParamString pString) : base(pString)
    {
    }

    public ParamMaterialPath()
    {

    }
}
