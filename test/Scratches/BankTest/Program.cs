// See https://aka.ms/new-console-template for more information

using System.Text;
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
var output = @"C:\Users\ryann\Downloads\Ryann\out\out.pbo";
File.Delete(output);
var outStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite);
var bank = RVBank.ReadPbo(@"C:\Users\ryann\Downloads\Ryann\HDSN_BreachingCharge.pbo", bankOptions, outStream, NullLogger.Instance);
bank.SynchronizeWithStream(bankOptions);
outStream.Close();
Console.WriteLine();

var outBank = RVBank.ReadPbo(output, bankOptions, null, NullLogger.Instance);

