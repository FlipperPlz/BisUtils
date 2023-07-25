namespace BisUtils.RVTexture.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRVTexture : IBinaryObject<RVTextureOptions>
{

}

public class RVTexture : BinaryObject<RVTextureOptions>, IRVTexture
{
    public RVTexture(BisBinaryReader reader, RVTextureOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RVTexture(ILogger? logger) : base(logger)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, RVTextureOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RVTextureOptions options) => throw new NotImplementedException();


}
