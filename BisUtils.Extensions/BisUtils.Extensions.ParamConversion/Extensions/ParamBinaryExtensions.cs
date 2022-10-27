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
                        foreach (var rapStatement in childClazz.Statements) writer.WriteOFPStatement(rapStatement);
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

    public static void FromBinary(this ParamFile paramFile, Stream stream,
        ParamFileBinaryFormats format = ParamFileBinaryFormats.RapElite) {
        var memStream = new MemoryStream();
        stream.CopyTo(memStream);
        memStream.Seek(0, SeekOrigin.Begin);

        using var reader = new BinaryReader(memStream);

        switch (format) {
            case ParamFileBinaryFormats.RapElite: {
                var bits = reader.ReadBytes(4);
                if (!(bits[0] == '\0' && bits[1] == 'r' && bits[2] == 'a' && bits[3] == 'P')) throw new Exception("Invalid header.");
                if(reader.ReadUInt32() != 0 || reader.ReadUInt32() != 8) throw new Exception("Expected bytes 0 and 8.");
                /*var enumOffset = */reader.ReadUInt32(); //TODO: Enums 0.o

                bool ReadParentClasses() {
                    reader.ReadAsciiZ();
                    var parentEntryCount = reader.ReadCompactInteger();

                    for (var i = 0; i < parentEntryCount; ++i) {
                        switch (reader.PeekChar()) {
                            case 0: paramFile.Statements.Add(reader.ReadOFPClassDeclaration());      break;
                            case 1: paramFile.Statements.Add(reader.ReadOFPVariableDeclaration());   break;
                            case 2: paramFile.Statements.Add(reader.ReadOFPArrayDeclaration());      break;
                            case 3: paramFile.Statements.Add(reader.ReadOFPRapExternalStatement());  break;
                            case 4: paramFile.Statements.Add(reader.ReadOFPRapDeleteStatement());    break;
                            case 5: paramFile.Statements.Add(reader.ReadOFPRapAppensionStatement()); break;
                            default: throw new NotSupportedException();
                        }
                    }
                    return parentEntryCount > 0;
                }

                void AddEntryToClass(RapClassDeclaration clazz) {
                    var entryType = reader.PeekChar();
                    switch (entryType) {
                        case 0: clazz.Statements.Add(reader.ReadOFPClassDeclaration());              return;
                        case 1: clazz.Statements.Add(reader.ReadOFPVariableDeclaration());           return;
                        case 2: clazz.Statements.Add(reader.ReadOFPArrayDeclaration());              return;
                        case 3: clazz.Statements.Add(reader.ReadOFPRapExternalStatement());          return;
                        case 4: clazz.Statements.Add(reader.ReadOFPRapDeleteStatement());            return;
                        case 5: clazz.Statements.Add(reader.ReadOFPRapAppensionStatement());         return;
                        default: throw new Exception();
                    }
                }
                
                bool ReadChildClasses() {
                    void LoadChildClasses(RapClassDeclaration clazz) {
                        reader.BaseStream.Position = clazz.BinaryOffset;
                        var parent = reader.ReadAsciiZ();
                        clazz.ParentClassname = (parent == string.Empty) ? null : parent;
                        for (var i = 0; i < reader.ReadCompactInteger(); ++i) AddEntryToClass(clazz);
                        
                        foreach (var statement in paramFile.Statements) {
                            if(statement is not RapClassDeclaration child) continue;
                            LoadChildClasses(child);
                        }
                    }
                    

                    var i = 0;
                    foreach (var statement in paramFile.Statements) {
                        if(statement is not RapClassDeclaration clazz) continue;
                        LoadChildClasses(clazz);
                        i++;
                    }
                    return i > 0;
                }

                if (!ReadParentClasses()) throw new Exception("No parent classes were found. (OFP:ParamBinaryExtensions)");
                if (!ReadChildClasses()) throw new Exception("No child classes were found. (OFP:ParamBinaryExtensions)");
                //TODO: Read Enums
                break;
            }
            case ParamFileBinaryFormats.RapOFP: throw new NotImplementedException();
            default: throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}