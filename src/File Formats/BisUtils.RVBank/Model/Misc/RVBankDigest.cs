// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace BisUtils.RVBank.Model.Misc;

using System.Runtime.InteropServices;
using Core.IO;
using Options;

public interface IRVBankDigest
{
    public int SectorA { get; }
    public int SectorB { get; }
    public int SectorC { get; }
    public int SectorD { get; }
    public int SectorE { get; }
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct RVBankDigest : IRVBankDigest
{
    public int SectorA { get; private init; }
    public int SectorB { get; private init; }
    public int SectorC { get; private init; }
    public int SectorD { get; private init; }
    public int SectorE { get; private init; }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public RVBankDigest(byte[] digest)
    {
        var ptrDigest = GCHandle.Alloc(digest, GCHandleType.Pinned);
        Marshal.PtrToStructure(ptrDigest.AddrOfPinnedObject(), this);
        ptrDigest.Free();
    }

    public void Write(BisBinaryWriter writer, RVBankOptions options)
    {
        writer.Write(SectorA);
        writer.Write(SectorB);
        writer.Write(SectorC);
        writer.Write(SectorD);
        writer.Write(SectorE);
    }

    public static bool operator ==(RVBankDigest obj1, RVBankDigest obj2) => obj1.Equals(obj2);
    public static bool operator !=(RVBankDigest obj1, RVBankDigest obj2) => !(obj1 == obj2);
    public bool Equals(RVBankDigest other) => SectorA.Equals(other.SectorA)
        && SectorB.Equals(other.SectorB)
        && SectorC.Equals(other.SectorC)
        && SectorD.Equals(other.SectorD)
        && SectorE.Equals(other.SectorE);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        return obj is RVBankDigest other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = SectorA.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorB.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorC.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorD.GetHashCode();
            hashCode = (hashCode * 397) ^ SectorE.GetHashCode();

            return hashCode;
        }
    }

}


