using System.Text;
using BisUtils.Extensions.ParamConversion.IO;
using BisUtils.Parsers.ParamParser;
using BisUtils.Parsers.ParamParser.Declarations;

namespace BisUtils.Extensions.ParamConversion; 

public static class ParamBinaryExtensions {
    public static MemoryStream ToBinary(this ParamFile paramFile,
        ParamFileBinaryFormats format = ParamFileBinaryFormats.RapElite) {
        var output = new MemoryStream();

        using var writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        switch (format) {
            case ParamFileBinaryFormats.RapElite: {
                void WriteParentClasses() {
                    writer.WriteAsciiZ();
                    writer.WriteCompactInteger(paramFile.Statements.Count);
                    foreach (var statement in paramFile.Statements) writer.WriteOFPStatement(statement);
                }

                void WriteChildClasses() {
                    void SaveChildClasses(RapClassDeclaration childClazz) {
                        childClazz.BinaryOffset = (uint) writer.BaseStream.Position;
                        writer.BaseStream.Position = childClazz.BinaryOffsetPosition;
                        writer.Write(BitConverter.GetBytes(childClazz.BinaryOffset), 0, 4);
                        writer.BaseStream.Position = childClazz.BinaryOffset;
                        writer.WriteAsciiZ(childClazz.ParentClassname ?? string.Empty);
                        writer.WriteCompactInteger(childClazz.Statements.ToList().Count);
                        foreach (var rapStatement in childClazz.Statements) 
                            writer.WriteOFPStatement(rapStatement);
                        foreach (var rapClass in childClazz.Statements) 
                            if(rapClass is RapClassDeclaration clazz) SaveChildClasses(clazz);
                    }
                        
                    foreach (var rapStatement in paramFile.Statements) {
                        if(rapStatement is not RapClassDeclaration childClazz) continue;
                        SaveChildClasses(childClazz);
                    }
                }
                    
                    
                writer.Write(new byte[] {0x00, (byte) 'r', (byte) 'a', (byte) 'P'});
                writer.Write((uint) 0);
                writer.Write((uint) 8);
                var enumOffsetPosition = writer.BaseStream.Position;
                writer.Write((uint) 999999); //Write Enum offset. will be changed later

                WriteParentClasses();
                WriteChildClasses();
                    
                var enumOffset = (uint) writer.BaseStream.Position;
                writer.BaseStream.Position = enumOffsetPosition;
                writer.Write(BitConverter.GetBytes(enumOffset), 0, 4);
                writer.BaseStream.Position = enumOffset;
        
                writer.Write((uint) 0);
                    
                break;
            }
            case ParamFileBinaryFormats.RapOFP: throw new NotImplementedException();
            default: throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
            
        writer.BaseStream.Seek(0, SeekOrigin.Begin);
        writer.BaseStream.CopyTo(output);
        output.Seek(0, SeekOrigin.Begin);
        writer.Dispose();

        return output;
    }
    
    
}