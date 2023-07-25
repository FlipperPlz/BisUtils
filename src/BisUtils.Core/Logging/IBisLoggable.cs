namespace BisUtils.Core.Logging;

using Microsoft.Extensions.Logging;

public interface IBisLoggable
{
    public ILogger? Logger { get; }
}
