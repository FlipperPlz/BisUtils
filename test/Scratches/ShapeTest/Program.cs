// See https://aka.ms/new-console-template for more information

using BisUtils.Core.IO;
using BisUtils.RVShape.Models;
using BisUtils.RVShape.Options;

var reader = new BisBinaryReader(File.OpenRead(@"C:\Users\ryann\Downloads\watches_m.p3d"));
var shapeOptions = new RVShapeOptions();
var shape = new RVShape("watches_m", reader, shapeOptions);
Console.WriteLine("Done");
