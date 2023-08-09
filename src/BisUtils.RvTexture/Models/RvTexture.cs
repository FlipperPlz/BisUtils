namespace BisUtils.RvTexture.Models;

using Core.Binarize;
using Core.Binarize.Implementation;
using Core.Extensions;
using Core.IO;
using FResults;
using Microsoft.Extensions.Logging;
using Options;

public interface IRvTexture : IBinaryObject<RvTextureOptions>
{

}

public class RvTexture : BinaryObject<RvTextureOptions>, IRvTexture
{
    public RvTexture(BisBinaryReader reader, RvTextureOptions options, ILogger? logger) : base(reader, options, logger)
    {
        if (!Debinarize(reader, options))
        {
            LastResult!.Throw();
        }
    }

    public RvTexture(ILogger? logger) : base(logger)
    {
    }

    public override Result Binarize(BisBinaryWriter writer, RvTextureOptions options) => throw new NotImplementedException();

    public sealed override Result Debinarize(BisBinaryReader reader, RvTextureOptions options) => throw new NotImplementedException();


}
