namespace BisUtils.DzServerQuery.Models;

public record DzServerMod(
    string ModName,
    long ModID,
    int ModHash,
    bool DLC
);
