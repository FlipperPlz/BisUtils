// See https://aka.ms/new-console-template for more information

using BisUtils.Core.IO;
using BisUtils.RvShape.Models;
using BisUtils.RvShape.Options;
using Microsoft.Extensions.Logging.Abstractions;

var reader = new BisBinaryReader(File.OpenRead(@"C:\Users\ryann\Desktop\breachingcharge.p3d"));
var shapeOptions = new RvShapeOptions();
var shape = new RvShape("breachingcharge", reader, shapeOptions, NullLogger.Instance);
Console.WriteLine("Done");
