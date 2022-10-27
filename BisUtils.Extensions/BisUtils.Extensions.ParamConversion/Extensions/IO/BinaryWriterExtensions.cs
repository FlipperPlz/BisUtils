using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Extensions.ParamConversion.IO; 

internal static class BinaryWriterExtensions {
    public static void WriteRapString(this BinaryWriter writer, string str) => writer.WriteAsciiZ(str);
    
    public static void WriteRapString(this BinaryWriter writer, RapString str) => writer.WriteAsciiZ(str.Value);
//  --------------------------------------------------------------------------------------------------------------------
    public static void WriteRapInteger(this BinaryWriter writer, int i) => writer.Write(i);
    
    public static void WriteRapInteger(this BinaryWriter writer, RapInteger rapInteger) => writer.Write(rapInteger.Value);
//  --------------------------------------------------------------------------------------------------------------------
    public static void WriteRapFloat(this BinaryWriter writer, float f) => writer.Write(f);
    
    public static void WriteRapFloat(this BinaryWriter writer, RapFloat f) => writer.Write(f.Value);
//  --------------------------------------------------------------------------------------------------------------------
    public static void WriteRapArray(this BinaryWriter writer, IEnumerable<IRapArrayEntry> entries) {
        var entriesList = entries.ToList();
        writer.WriteCompactInteger(entriesList.Count);

        foreach (var entry in entriesList) {
            switch (entry) {
                case RapString @string: {
                    writer.Write((byte) 0);
                    writer.WriteRapString(@string);
                    continue;
                };
                case RapFloat @float: {
                    writer.Write((byte) 1);
                    writer.WriteRapFloat(@float);
                    continue;
                };
                case RapInteger @int: {
                    writer.Write((byte) 2);
                    writer.WriteRapInteger(@int);
                    continue;
                };
                case RapArray array: {
                    writer.Write((byte) 3);
                    writer.WriteRapArray(array);
                    continue;
                };
                default: throw new NotSupportedException();
            }
        }
    }
    
    public static void WriteRapArray(this BinaryWriter writer, RapArray array) => writer.WriteRapArray(array.Entries);
//  --------------------------------------------------------------------------------------------------------------------
    public static void WriteOFPRapAppensionStatement(this BinaryWriter writer, RapAppensionStatement statement) {
        writer.Write((byte) 5);
        writer.Write((int) 1);
        writer.WriteAsciiZ(statement.Target);
        writer.WriteRapArray(statement.Array);
    }
    
    public static void WriteOFPRapDeleteStatement(this BinaryWriter writer, RapDeleteStatement statement) {
        writer.Write((byte) 4);
        writer.WriteAsciiZ(statement.Target);
    }
    
    public static void WriteOFPRapExternalClassStatement(this BinaryWriter writer, RapExternalClassStatement statement) {
        writer.Write((byte) 3);
        writer.WriteAsciiZ(statement.Classname);
    }
//  --------------------------------------------------------------------------------------------------------------------
    public static void WriteOFPArrayDeclaration(this BinaryWriter writer, RapArrayDeclaration declaration) {
        writer.Write((byte) 2);
        writer.WriteAsciiZ(declaration.ArrayName);
        writer.WriteRapArray(declaration.ArrayValue);
    }
    
    public static void WriteOFPVariableDeclaration(this BinaryWriter writer, RapVariableDeclaration declaration) {
        writer.Write((byte) 1);
        switch (declaration.VariableValue) {
            case RapString @string:
                writer.Write((byte) 0);
                writer.WriteAsciiZ(declaration.VariableName);
                writer.WriteRapString(@string);
                break;
            case RapFloat @float:
                writer.Write((byte) 1);
                writer.WriteAsciiZ(declaration.VariableName);
                writer.WriteRapFloat(@float);
                break;
            case RapInteger @int:
                writer.Write((byte) 2);
                writer.WriteAsciiZ(declaration.VariableName);
                writer.WriteRapInteger(@int);
                break;
            default: throw new NotSupportedException();
        }
    }

    public static void WriteOFPClassDeclaration(this BinaryWriter writer, RapClassDeclaration declaration) {
        writer.Write((byte) 0);
        writer.WriteAsciiZ(declaration.Classname);
        declaration.BinaryOffsetPosition = writer.BaseStream.Position;
        writer.Write((uint) declaration.BinaryOffset);
    }

    public static void WriteOFPStatement(this BinaryWriter writer, IRapStatement statement) {
        switch (statement) {
            case RapClassDeclaration clazz: 
                WriteOFPClassDeclaration(writer, clazz);
                break;
            case RapArrayDeclaration array:
                WriteOFPArrayDeclaration(writer, array);
                break;
            case RapVariableDeclaration var:
                WriteOFPVariableDeclaration(writer, var);
                break;
            case RapExternalClassStatement external:
                WriteOFPRapExternalClassStatement(writer, external);
                break;
            case RapDeleteStatement delete:
                WriteOFPRapDeleteStatement(writer, delete);
                break;
            case RapAppensionStatement append:
                WriteOFPRapAppensionStatement(writer, append);
                break;
            default: throw new Exception($"No logic for {statement.GetType().Name} (BinaryWriterExtensions::WriteOFPStatement {{ParamConversion}} )");
        }
    }

}