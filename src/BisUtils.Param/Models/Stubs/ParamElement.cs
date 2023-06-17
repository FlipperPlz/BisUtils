namespace BisUtils.Param.Models.Stubs;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Family;
using Core.IO;
using Options;

public interface IParamElement : IFamilyMember, IStrictBinaryObject<ParamOptions>
{
    IParamFile? ParamFile { get; }
    IFamilyNode? IFamilyMember.Node => ParamFile;
}

public abstract class ParamElement : StrictBinaryObject<ParamOptions>, IParamElement
{
    public IFamilyNode? Node => ParamFile;
    public IParamFile? ParamFile { get; set; }

    protected ParamElement(IParamFile? file) : base()
    {
    }

    protected ParamElement(BisBinaryReader reader, ParamOptions options) : base(reader, options)
    {

    }

}
