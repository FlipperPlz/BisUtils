namespace BisUtils.RVTexture.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.IO;
using FResults;
using Options;

public interface IRVTexture : IBinaryObject<RVTextureOptions>
{

}

public class RVTexture : BinaryObject<RVTextureOptions>, IRVTexture
{
    public override Result Binarize(BisBinaryWriter writer, RVTextureOptions options) => throw new NotImplementedException();

    public override Result Debinarize(BisBinaryReader reader, RVTextureOptions options) => throw new NotImplementedException();
}
