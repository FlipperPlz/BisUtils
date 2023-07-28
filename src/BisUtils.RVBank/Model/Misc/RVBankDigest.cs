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

public struct RVBankDigest : IRVBankDigest
{
    public int SectorA { get; private set; }
    public int SectorB { get; private set; }
    public int SectorC { get; private set; }
    public int SectorD { get; private set; }
    public int SectorE { get; private set; }


    public RVBankDigest(BisBinaryReader reader) => Read(reader);

    public RVBankDigest(byte[] c)
    {
        if (c.Length < 20)
        {
            throw new ArgumentException("`c` must be at least 20 bytes.", nameof(c));
        }

        SectorA = BitConverter.ToInt32(c, 0);
        SectorB = BitConverter.ToInt32(c, 4);
        SectorC = BitConverter.ToInt32(c, 8);
        SectorD = BitConverter.ToInt32(c, 12);
        SectorE = BitConverter.ToInt32(c, 16);
    }

    public void Read(BisBinaryReader reader)
    {
        SectorA = reader.ReadInt32();
        SectorB = reader.ReadInt32();
        SectorC = reader.ReadInt32();
        SectorD = reader.ReadInt32();
        SectorE = reader.ReadInt32();
    }

    public byte[] ToByteArray()
    {
        var byteArray = new byte[20];

        WriteBytes(SectorA, 0);
        WriteBytes(SectorB, 4);
        WriteBytes(SectorC, 8);
        WriteBytes(SectorD, 12);
        WriteBytes(SectorE, 16);

        return byteArray;

        void WriteBytes(int value, int start)
        {
            byteArray[start] = (byte)value;
            byteArray[start + 1] = (byte)(value >> 8);
            byteArray[start + 2] = (byte)(value >> 16);
            byteArray[start + 3] = (byte)(value >> 24);
        }
    }

    public void Write(BisBinaryWriter writer)
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


