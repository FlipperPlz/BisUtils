namespace BisUtils.DZServerQuery.Models;

public record DzServerMod(
    string ModName,
    long ModID,
    int ModHash,
    bool DLC
);
