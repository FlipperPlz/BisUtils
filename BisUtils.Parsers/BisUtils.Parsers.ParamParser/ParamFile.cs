using System.Text;
using Antlr4.Runtime.Misc;
using BisUtils.Core.Serialization;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;
using BisUtils.Parsers.ParamParser.Utils.Factories;
using BisUtils.Parsers.ParamParser.Utils.Interfaces;

namespace BisUtils.Parsers.ParamParser; 

public class ParamFile : IParamParsable<Generated.ParamLang.ParamParser.ComputationalStartContext, ParamFile>{
    public List<ParamStatement> Statements { get; set; } = new();
    public Dictionary<string, int?>? EnumValues { get; set; } = null;
    
    public ParamFile FromParserContext(Generated.ParamLang.ParamParser.ComputationalStartContext ctx) {
        if (ctx.statement() is { } statements) Statements.AddRange(statements.Select(ParamStatementFactory.Create));
        if (ctx.enumDeclaration() is { } enums) {
            EnumValues = new Dictionary<string, int?>();
            foreach (var e in enums) {
                foreach (var enumValueContext in e.enumValue()) {

                    EnumValues.Add(ctx.Start.InputStream.GetText(new Interval(
                            enumValueContext.identifier().Start.StartIndex,
                            enumValueContext.identifier().Stop.StopIndex)),
                        enumValueContext.literalInteger() is { } integerContext
                            ? new ParamLiteralInteger(integerContext).Value
                            : null);

                }
            }
        }
        return this;
    }

    public void WriteString(StringBuilder builder, ParamSerializationOptions serializationOptions) {
        var statements = Statements;
        if(serializationOptions.OrganizeEntries) Statements.Sort((x, y) => x.CompareTo(y));
        foreach (var s in statements) {
            ParamStatementFactory.CreateSerializable(s).WriteString(builder, ParamSerializationOptions.Defaults);
            builder.Append('\n');
        }
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
                var bits = reader.ReadBytes(4);
        if (!(bits[0] == '\0' && bits[1] == 'r' && bits[2] == 'a' && bits[3] == 'P'))
            throw new Exception("Invalid header.");
        if (reader.ReadUInt32() != 0 || reader.ReadUInt32() != 8) throw new Exception("Expected bytes 0 and 8.");
        /*var enumOffset = */
        reader.ReadUInt32(); //TODO: Enums 0.o

        bool ReadParentClasses() {
            reader.ReadAsciiZ();
            var parentEntryCount = reader.ReadCompactInteger();

            for (var i = 0; i < parentEntryCount; i++) {
                switch (reader.PeekChar()) {
                    case 0:
                        Statements.Add(reader.ReadBinarized<ParamClassDeclaration>());
                        break;
                    case 1:
                        Statements.Add(reader.ReadBinarized<ParamVariableDeclaration>());
                        break;
                    case 2:
                        Statements.Add(reader.ReadBinarized<ParamArrayDeclaration>());
                        break;
                    case 3:
                        Statements.Add(reader.ReadBinarized<ParamExternalClassStatement>());
                        break;
                    case 4:
                        Statements.Add(reader.ReadBinarized<ParamDeleteStatement>());
                        break;
                    case 5:
                        Statements.Add(reader.ReadBinarized<ParamAppensionStatement>());
                        break;
                    default: throw new NotSupportedException();
                }
            }

            return parentEntryCount > 0;
        }

        void AddEntryToClass(ParamClassDeclaration clazz) {
            var entryType = reader.PeekChar();
            switch (entryType) {
                case 0:
                    Statements.Add(reader.ReadBinarized<ParamClassDeclaration>());
                    break;
                case 1:
                    Statements.Add(reader.ReadBinarized<ParamVariableDeclaration>());
                    break;
                case 2:
                    Statements.Add(reader.ReadBinarized<ParamArrayDeclaration>());
                    break;
                case 3:
                    Statements.Add(reader.ReadBinarized<ParamExternalClassStatement>());
                    break;
                case 4:
                    Statements.Add(reader.ReadBinarized<ParamDeleteStatement>());
                    break;
                case 5:
                    Statements.Add(reader.ReadBinarized<ParamAppensionStatement>());
                    break;
                default: throw new Exception();
            }
        }

        bool ReadChildClasses() {
            void LoadChildClasses(ParamClassDeclaration clazz) {
                reader.BaseStream.Position = clazz.BinaryOffset;
                var parent = reader.ReadAsciiZ();
                clazz.ParentClassname = (parent == string.Empty) ? null : parent;
                var clazzCount = reader.ReadCompactInteger();
                for (var i = 0; i < clazzCount; i++) AddEntryToClass(clazz);

                foreach (var statement in clazz.Statements) {
                    if (statement is not ParamClassDeclaration child) continue;
                    LoadChildClasses(child);
                }
            }


            var i = 0;
            foreach (var statement in Statements) {
                if (statement is not ParamClassDeclaration clazz) continue;
                LoadChildClasses(clazz);
                i++;
            }

            return i > 0;
        }

        if (!ReadParentClasses()) throw new Exception("No parent classes were found. (OFP:ParamBinaryExtensions)");
        if (!ReadChildClasses()) throw new Exception("No child classes were found. (OFP:ParamBinaryExtensions)");
        reader.ReadInt32();
        var enumCount = reader.ReadInt32();
        if (enumCount == 0) return this;

        EnumValues = new Dictionary<string, int?>();

        for (var e = 0; e < enumCount; e++) EnumValues.Add(reader.ReadAsciiZ(), reader.ReadInt32());
        return this;
    }

    public void WriteBinary(BinaryWriter writer) { 
        void WriteParentClasses() {
            writer.WriteAsciiZ();
            writer.WriteCompactInteger(Statements.Count);
            foreach (var statement in Statements) ParamStatementFactory.CreateBinarizable(statement).WriteBinary(writer);
        }

        void WriteChildClasses() {
            void SaveChildClasses(ParamClassDeclaration childClazz) {
                childClazz.BinaryOffset = (uint)writer.BaseStream.Position;
                writer.BaseStream.Position = childClazz.BinaryOffsetPosition;
                writer.Write(BitConverter.GetBytes(childClazz.BinaryOffset), 0, 4);
                writer.BaseStream.Position = childClazz.BinaryOffset;
                writer.WriteAsciiZ(childClazz.ParentClassname ?? string.Empty);
                writer.WriteCompactInteger(childClazz.Statements.ToList().Count);
                foreach (var rapStatement in childClazz.Statements) ParamStatementFactory.CreateBinarizable(rapStatement).WriteBinary(writer);
                foreach (var rapClass in childClazz.Statements)
                    if (rapClass is ParamClassDeclaration clazz)
                        SaveChildClasses(clazz);
            }

            foreach (var rapStatement in Statements) {
                if (rapStatement is not ParamClassDeclaration childClazz) continue;
                SaveChildClasses(childClazz);
            }
        }


        writer.Write(new byte[] { 0x00, (byte)'r', (byte)'a', (byte)'P' });
        writer.Write((uint)0);
        writer.Write((uint)8);
        var enumOffsetPosition = writer.BaseStream.Position;
        writer.Write((uint)999999); //Write Enum offset. will be changed later

        WriteParentClasses();
        WriteChildClasses();

        var enumOffset = (uint)writer.BaseStream.Position;
        writer.BaseStream.Position = enumOffsetPosition;
        writer.Write(BitConverter.GetBytes(enumOffset), 0, 4);
        writer.BaseStream.Position = enumOffset;
        writer.Write(766);

        writer.Write(EnumValues is { } vals ? vals.Count : 0);

        if (EnumValues is not null) {
            foreach (var param in EnumValues) {
                writer.WriteAsciiZ(param.Key);
                writer.Write(param.Value ?? 0);
            }
        }
    }
}