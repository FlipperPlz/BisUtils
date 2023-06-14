﻿using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

public abstract class PboVFSEntry : PboElement, IFamilyChild
{
    public PboDirectory? ParentDirectory { get; set; }
    public IFamilyParent? Parent => ParentDirectory;

    public string EntryName { get; set; } = string.Empty;

    public string Path => ParentDirectory?.Path + "\\" + EntryName;
    public string AbsolutePath => ParentDirectory?.AbsolutePath + "\\" + EntryName;

    protected PboVFSEntry(string entryName) : base() => EntryName = entryName;

    protected PboVFSEntry(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override BinarizationResult Debinarize(BisBinaryReader reader, PboOptions options)
    {
        var result = reader.ReadAsciiZ(out var readName, options) ; //TODO: Add error
        if (result.IsNotValid)
        {
            return result;
        }

        EntryName = readName;
        return result;
    }

    public override BinarizationResult Binarize(BisBinaryWriter writer, PboOptions options)
    {
        //TODO: Write AsciiZ
        throw new NotImplementedException();
    }
}
