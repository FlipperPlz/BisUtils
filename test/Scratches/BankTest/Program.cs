﻿using System.Text;
using BisUtils.Extensions.RVBank.Extras;
using BisUtils.RVBank.Extensions;
using BisUtils.RVBank.Model;
using BisUtils.RVBank.Options;
using Microsoft.Extensions.Logging.Abstractions;

var bankOptions = new RVBankOptions()
{
    AllowObfuscated = true,
    FlatRead = false,
    RemoveBenignProperties = true,
    AllowMultipleVersion = true,
    IgnoreValidation = true,
    DecompressionTimeout = TimeSpan.FromMinutes(2),
    Charset = Encoding.UTF8,
    RequireValidSignature = false,
    RegisterEmptyEntries = false,
    CompressionErrorsAreWarnings = false,
    RequireEmptyVersionMeta = false,
    RequireVersionMimeOnVersion = false,
    RequireFirstEntryIsVersion = false,
    RequireVersionNotNamed = false,
    AllowVersionMimeOnData = true,
    IgnoreInvalidStreamSize = true,
    AlwaysSeparateOnDummy = true,
    AllowUnnamedDataEntries = true,
    IgnoreDuplicateFiles = true,
    RespectEntryOffsets = false,
    WriteValidOffsets = false,
    AsciiLengthTimeout = 510,
    AllowEncrypted = true
};
var output = @"C:\Users\ryann\Downloads\Ryann\LBMaster\out2w.pbo";
File.Delete(output);
File.Delete(@"C:\Users\ryann\Desktop\test.cpp");
var outStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite);
var bank = new RVBank(@"C:\Users\ryann\Downloads\Ryann\LBMaster\AdvancedGroups_Server.pbo", bankOptions, outStream, NullLogger.Instance);
foreach (var treeLevel in bank.DirectoryTree())
{
    Console.WriteLine(treeLevel);
}
bank.SynchronizeWithStream(bankOptions);
var config = bank.GetDataEntries("config.cpp", SearchOption.TopDirectoryOnly).FirstOrDefault() ??
             throw new IOException("No config.cpp entry found.");
File.WriteAllBytes(@"C:\Users\ryann\Desktop\test.cpp", config.EntryData.ToArray());


outStream.Close();
Console.WriteLine();

var outBank = new RVBank(output, bankOptions, null, NullLogger.Instance);
Console.WriteLine();
