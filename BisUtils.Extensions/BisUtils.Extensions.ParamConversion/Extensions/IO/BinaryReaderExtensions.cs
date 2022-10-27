using System.IO;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Extensions.ParamConversion.IO; 

internal static class BinaryReaderExtensions {
    
    public static RapString ReadRapString(this BinaryReader reader) => new(reader.ReadAsciiZ());
    
    public static RapInteger ReadRapInteger(this BinaryReader reader) => new(reader.ReadInt32());
    
    public static RapFloat ReadRapFloat(this BinaryReader reader) => new(reader.ReadSingle());

    public static RapArray ReadRapArray(this BinaryReader reader) {
        var entries = new List<IRapArrayEntry>(reader.ReadCompactInteger());
        
        for (var i = 0; i < entries.Capacity; ++i) {
            switch (reader.ReadByte()) {
                case 0: { // String
                    entries.Add(reader.ReadRapString());
                    break;
                };
                case 1: { // Float
                    entries.Add(reader.ReadRapFloat());
                    break;
                };
                case 2: { // Integer
                    entries.Add(reader.ReadRapInteger());
                    break;
                };
                case 3: { // Child Array
                    entries.Add(reader.ReadRapArray());
                    break;
                };
                case 4: // Variable
                default: {
                    throw new Exception();
                };
            }
        }
        return new RapArray(entries);
    }
    
    public static RapAppensionStatement ReadOFPRapAppensionStatement(this BinaryReader reader) {
        if (reader.ReadByte() != 5) throw new Exception("Expected array appension.");
        if (reader.ReadInt32() != 1) throw new Exception("Expected array appension. (1)");
        return new RapAppensionStatement(reader.ReadAsciiZ(), reader.ReadRapArray());
    }
    
    public static RapDeleteStatement ReadOFPRapDeleteStatement(this BinaryReader reader) {
        if (reader.ReadByte() != 4) throw new Exception("Expected delete statement.");
        return new RapDeleteStatement(reader.ReadAsciiZ());
    }
    
    public static RapExternalClassStatement ReadOFPRapExternalStatement(this BinaryReader reader) {
        if (reader.ReadByte() != 3) throw new Exception("Expected external class.");
        return new RapExternalClassStatement(reader.ReadAsciiZ());
    }
    
    public static RapArrayDeclaration ReadOFPArrayDeclaration(this BinaryReader reader) {
        if (reader.ReadByte() != 2) throw new Exception("Expected external class.");
        return new RapArrayDeclaration(reader.ReadAsciiZ(), reader.ReadRapArray());
    }
    
    public static RapVariableDeclaration ReadOFPVariableDeclaration(this BinaryReader reader) {
        if (reader.ReadByte() != 1) throw new Exception("Expected token.");
        var valType = reader.ReadByte();
        var variableName = reader.ReadAsciiZ();
        IRapLiteral variableValue = valType switch {
            0 => reader.ReadRapString(),
            1 => reader.ReadRapFloat(),
            2 => reader.ReadRapInteger(),
            _ => throw new Exception()
        };
        return new RapVariableDeclaration(variableName, variableValue);
    }

    public static RapClassDeclaration ReadOFPClassDeclaration(this BinaryReader reader) {
        if ( reader.ReadByte() != 0) throw new Exception($"Expected class.");
        var clazz = new RapClassDeclaration(reader.ReadAsciiZ()) {
            BinaryOffsetPosition = reader.BaseStream.Position,
            BinaryOffset = reader.ReadUInt32()
        };

        return clazz;
    }

}