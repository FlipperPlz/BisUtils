// See https://aka.ms/new-console-template for more information

using BisUtils.EnfPack.Models;
using BisUtils.EnfPack.Options;
using Microsoft.Extensions.Logging.Abstractions;

var packOptions = new EsPackOptions() { };
var pack = new EsPackFile(@"C:\Users\ryann\Downloads\Ryann\LBMaster\AdvancedGroups_Server.pbo", packOptions, null, NullLogger.Instance);
Console.WriteLine();
