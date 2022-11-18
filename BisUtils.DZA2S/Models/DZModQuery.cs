using System.Collections.ObjectModel;

namespace BisUtils.DZA2S.Models;

public record DZWorkshopMod(
    string ModName,
    bool IsDLC,
    long ModId,
    int ModHash);

public record DZModQuery(
    int Version,
    int Difficulty,
    short DlcFlags,
    DZWorkshopMod[] Mods
    );