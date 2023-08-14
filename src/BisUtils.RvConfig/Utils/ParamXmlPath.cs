namespace BisUtils.RvConfig.Utils;

using Core.IO;
using Enumerations;
using Models;
using Models.Literals;
using Models.Stubs.Holders;
using Options;

public class ParamXmlPath : ParamPathString
{
    public const string XmlExtension = "xml";
    public string DefaultExtension { get; } = "xml";
    public string[] AcceptableExtensions { get; } = new[] { "xml" };

    public ParamXmlPath(IParamString pString) : base(pString)
    {
    }

    public ParamXmlPath()
    {

    }

}
