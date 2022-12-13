using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using BisUtils.Core;
using BisUtils.Core.Serialization;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Parsers.ParamParser; 

public class ParamFile : IRapDeserializable<Generated.ParamLang.ParamParser.ComputationalStartContext> {
    public List<IRapStatement> Statements { get; set; } = new();
    public Dictionary<string, int?>? EnumValues { get; set; } = null;
   
    public static ParamFile ParseParamFile(Stream stream) {
        var memStream = new MemoryStream();
        stream.Seek(0, SeekOrigin.Begin);
        stream.CopyTo(memStream);
        memStream.Seek(0, SeekOrigin.Begin);
        using (var reader = new BinaryReader(memStream, Encoding.UTF8, true)) {
            var bits = reader.ReadBytes(4);
            if ((bits[0] == '\0' && bits[1] == 'r' && bits[2] == 'a' && bits[3] == 'P')) {
                memStream.Seek(0, SeekOrigin.Begin);
                return (ParamFile) new ParamFile().ReadBinary(reader);
                
            }
                
            memStream.Seek(0, SeekOrigin.Begin);
        }

        var lexer = new ParamLexer(CharStreams.fromStream(memStream));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);
           
        var computationalStart = parser.computationalStart();
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();

        return (ParamFile) new ParamFile().ReadParseTree(computationalStart);
    }
    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.ComputationalStartContext ctx) {
        if (ctx.statement() is { } statements) Statements.AddRange(statements.Select(RapStatementFactory.Create));
        if (ctx.enumDeclaration() is { } enums) {
            EnumValues = new Dictionary<string, int?>();
            foreach (var e in enums) {
                foreach (var enumValueContext in e.enumValue()) {

                    EnumValues.Add(ctx.Start.InputStream.GetText(new Interval(
                            enumValueContext.identifier().Start.StartIndex,
                            enumValueContext.identifier().Stop.StopIndex)),
                        enumValueContext.literalInteger() is { } integerContext
                            ? RapInteger.FromContext(integerContext).Value
                            : null);

                }
            }
        }
        return this;
    }

    public IBisBinarizable FromString(StringBuilder builder, RapDeserializationOptions deserializationOptions) {
        var lexer = new ParamLexer(CharStreams.fromString(builder.ToString()));
        var tokens = new CommonTokenStream(lexer);
        var parser = new Generated.ParamLang.ParamParser(tokens);

        ReadParseTree(parser.computationalStart());
        if (parser.NumberOfSyntaxErrors != 0) throw new Exception();
        
        return this;
    }

    public void Write(StringBuilder builder, RapSerializationOptions serializationOptions) {
        var statements = Statements;
        if(serializationOptions.OrganizeEntries) Statements.Sort((x, y) => x.CompareTo(y));
        foreach (var s in statements) {
            s.Write(builder, RapSerializationOptions.DefaultOptions);
            builder.Append('\n');
        }
    }

    public IBisBinarizable ReadBinary(BinaryReader reader) {
        //TODO: Broken can't be fucked to fix in this commit
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
                        Statements.Add(reader.ReadBinarized<RapClassDeclaration>());
                        break;
                    case 1:
                        Statements.Add(reader.ReadBinarized<RapVariableDeclaration>());
                        break;
                    case 2:
                        Statements.Add(reader.ReadBinarized<RapArrayDeclaration>());
                        break;
                    case 3:
                        Statements.Add(reader.ReadBinarized<RapExternalClassStatement>());
                        break;
                    case 4:
                        Statements.Add(reader.ReadBinarized<RapDeleteStatement>());
                        break;
                    case 5:
                        Statements.Add(reader.ReadBinarized<RapAppensionStatement>());
                        break;
                    default: throw new NotSupportedException();
                }
            }

            return parentEntryCount > 0;
        }

        void AddEntryToClass(RapClassDeclaration clazz) {
            var entryType = reader.PeekChar();
            switch (entryType) {
                case 0:
                    Statements.Add(reader.ReadBinarized<RapClassDeclaration>());
                    break;
                case 1:
                    Statements.Add(reader.ReadBinarized<RapVariableDeclaration>());
                    break;
                case 2:
                    Statements.Add(reader.ReadBinarized<RapArrayDeclaration>());
                    break;
                case 3:
                    Statements.Add(reader.ReadBinarized<RapExternalClassStatement>());
                    break;
                case 4:
                    Statements.Add(reader.ReadBinarized<RapDeleteStatement>());
                    break;
                case 5:
                    Statements.Add(reader.ReadBinarized<RapAppensionStatement>());
                    break;
                default: throw new Exception();
            }
        }

        bool ReadChildClasses() {
            void LoadChildClasses(RapClassDeclaration clazz) {
                reader.BaseStream.Position = clazz.BinaryOffset;
                var parent = reader.ReadAsciiZ();
                clazz.ParentClassname = (parent == string.Empty) ? null : parent;
                var clazzCount = reader.ReadCompactInteger();
                for (var i = 0; i < clazzCount; i++) AddEntryToClass(clazz);

                foreach (var statement in clazz.Statements) {
                    if (statement is not RapClassDeclaration child) continue;
                    LoadChildClasses(child);
                }
            }


            var i = 0;
            foreach (var statement in Statements) {
                if (statement is not RapClassDeclaration clazz) continue;
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
            foreach (var statement in Statements) statement.WriteBinary(writer);
        }

        void WriteChildClasses() {
            void SaveChildClasses(RapClassDeclaration childClazz) {
                childClazz.BinaryOffset = (uint)writer.BaseStream.Position;
                writer.BaseStream.Position = childClazz.BinaryOffsetPosition;
                writer.Write(BitConverter.GetBytes(childClazz.BinaryOffset), 0, 4);
                writer.BaseStream.Position = childClazz.BinaryOffset;
                writer.WriteAsciiZ(childClazz.ParentClassname ?? string.Empty);
                writer.WriteCompactInteger(childClazz.Statements.ToList().Count);
                foreach (var rapStatement in childClazz.Statements) rapStatement.WriteBinary(writer);
                foreach (var rapClass in childClazz.Statements)
                    if (rapClass is RapClassDeclaration clazz)
                        SaveChildClasses(clazz);
            }

            foreach (var rapStatement in Statements) {
                if (rapStatement is not RapClassDeclaration childClazz) continue;
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