﻿using BisUtils.Core.Binarize.Utils;
using BisUtils.Core.Family;
using BisUtils.Core.IO;

namespace BisUtils.Bank.Model.Stubs;

public class PboDirectory : PboVFSEntry, IFamilyParent
{
    public PboDirectory? ParentDirectory { get; set; }
    public IFamilyParent? Parent => ParentDirectory;
    
 
    public readonly List<PboEntry> PboEntries = new();

    public IEnumerable<PboVFSEntry> VfsEntries => PboEntries.OfType<PboVFSEntry>().ToList();
    public IEnumerable<IFamilyMember> Children => VfsEntries;

    public PboDirectory(List<PboEntry> entries) : base() => PboEntries = entries;
    
    public PboDirectory(BisBinaryReader reader, PboOptions options) : base(reader, options)
    {
    }

    public override BinarizationResult Binarize(BisBinaryWriter writer, PboOptions options)
    {
        var currentResult = BinarizationResult.Okay;
        //TODO: Append errors to current result, crash on non-recoverable errors
        foreach (var entry in PboEntries) entry.Binarize(writer, options); 

        return currentResult;
    }

    public override BinarizationResult Debinarize(BisBinaryReader reader, PboOptions options) =>
        throw new NotSupportedException();

    public override bool Validate(PboOptions options) => PboEntries.TrueForAll(e => e.Validate(options));

}