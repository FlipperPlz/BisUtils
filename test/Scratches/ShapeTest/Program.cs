// See https://aka.ms/new-console-template for more information

using BisUtils.Core.IO;
using BisUtils.P3D.Models;
using BisUtils.P3D.Options;

var reader = new BisBinaryReader(File.OpenRead(@"C:\Users\ryann\Desktop\box.p3d"));
var shapeOptions = new RVShapeOptions();
var shape = new RVShape("box", reader, shapeOptions);
Console.WriteLine("Done");
