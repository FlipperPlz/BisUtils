// See https://aka.ms/new-console-template for more information

using BisUtils.EnPack.Models;
using BisUtils.EnPack.Options;
using Microsoft.Extensions.Logging.Abstractions;

var packOptions = new EnPackOptions() { };
var pack = new EnPackFile(@"C:\Users\ryann\Downloads\Ryann\LBMaster\AdvancedGroups_Server.pbo", packOptions, null, NullLogger.Instance);
Console.WriteLine();
