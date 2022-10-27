using System.Text;
using Antlr4.Runtime;
using BisUtils.Extensions.ParamConversion.Enumeration;
using BisUtils.Generated.ParamLang;
using BisUtils.Parsers.ParamParser;
using BisUtils.Parsers.ParamParser.Declarations;
using BisUtils.Parsers.ParamParser.Interfaces;
using BisUtils.Parsers.ParamParser.Literals;
using BisUtils.Parsers.ParamParser.Statements;

namespace BisUtils.Extensions.ParamConversion;

public static class ParamConversionExtensions {
    public static string ToString(this ParamFile paramFile, ParamFileTextFormats format = ParamFileTextFormats.CPP) {
        switch (format) {
            case ParamFileTextFormats.CPP: {
                var builder = new StringBuilder(string.Join('\n', paramFile.Statements.Select(s => s.ToString())));
                builder.Append("\nenum {\n");
                var keyList = paramFile.EnumValues.Keys.ToList();
                var valList = paramFile.EnumValues.Values.ToList();

                for (var i = 0; i < keyList.Count; i++) {
                    for (var j = 0; j < valList.Count; j++) {
                        var key = keyList[i];
                        var val = valList[j];
                        builder.Append(key);
                        if (val is not null) builder.Append('=').Append(val.Value);
                        if (i + 1 < keyList.Count) builder.Append(',');
                        builder.Append('\n');
                    }
                }

                builder.Append("};\n");
                return builder.ToString();
            }
            case ParamFileTextFormats.XML: {
                var builder = new StringBuilder("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>\n");
                builder.Append("<!-- BIS Config File -->\n");
                foreach (var statement in paramFile.Statements) builder.Append(WriteParamXML(statement)).Append('\n');
                return builder.ToString();
            }
            default: throw new NotSupportedException();
        }
    }

    public static string[]? FromString(this ParamFile paramFile, Stream paramsStream, ParamFileTextFormats format = ParamFileTextFormats.CPP) {
        var stream = new MemoryStream();
        paramsStream.CopyTo(stream);
        stream.Seek(0, SeekOrigin.Begin);
        
        switch (format) {
            case ParamFileTextFormats.CPP: {
                var lexer = new ParamLexer(CharStreams.fromStream(stream));
                var tokens = new CommonTokenStream(lexer);
                var parser = new ParamParser(tokens);
           
                var computationalStart = parser.computationalStart();
                if (parser.NumberOfSyntaxErrors != 0) throw new Exception(); //TODO return error list
                paramFile.ReadParseTree(computationalStart);
                
                return null;
            }
            case ParamFileTextFormats.XML: throw new NotImplementedException();
            default: throw new NotSupportedException();
        }
    }

    private static string WriteParamXML(IRapSerializable token, int indentation = char.MinValue, bool asArrVal = false) {
        var builder = new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)));
        
        switch (token) {
            case RapClassDeclaration rapClass: {
                builder.Append($"<{rapClass.Classname}");
                if (rapClass.ParentClassname is not null) builder.Append($" base=\"{rapClass.ParentClassname}\"");
                builder.Append('>').Append('\n');
                foreach (var rapClassStatement in rapClass.Statements) 
                    builder.Append(WriteParamXML(rapClassStatement, indentation + 1)).Append('\n');
                builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", indentation))).Append("</")
                    .Append(rapClass.Classname).Append('>');
                return builder.ToString();
            }
            case RapExternalClassStatement rapExternalClassStatement: 
                return builder.Append($"<{rapExternalClassStatement.Classname}></{rapExternalClassStatement.Classname}>\n").ToString();
            case RapArrayDeclaration arrayDeclaration: {
                builder.Append($"<{arrayDeclaration.ArrayName} type=\"array\">\n");
                builder.Append(WriteParamXML(arrayDeclaration.ArrayValue, indentation + 1, false)).Append('\n');
                return builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
                    .Append($"</{arrayDeclaration.ArrayName}>\n").ToString();
            }
            case RapAppensionStatement appensionStatement: { //WAITING/DEPRECATED: BIS has not updated this format to support appension... lame
                builder.Append($"<{appensionStatement.Target} type=\"array\">\n");
                builder.Append(WriteParamXML(appensionStatement.Array, indentation + 1, false)).Append('\n');
                return builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
                    .Append($"</{appensionStatement.Target}>\n").ToString();
            }
            case RapVariableDeclaration variableDeclaration:
                return builder.Append($"<{variableDeclaration.VariableName}>")
                    .Append(WriteParamXML(variableDeclaration.VariableValue, char.MinValue, false))
                    .Append($"</{variableDeclaration.VariableName}>\n").ToString();
            case RapDeleteStatement: return builder.ToString(); //WAITING/DEPRECATED: BIS has not updated this format to support delete... lame
            case RapArray rapArray: {
                switch (asArrVal) {
                    case true: {
                        builder.Append("<item type=\"array\">\n");
                        foreach (var rapArrayEntry in rapArray.Entries) 
                            builder.Append(WriteParamXML(rapArrayEntry, indentation + 1, true)).Append('\n');
                        builder.Append(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
                            .Append("</item>\n");
                        return builder.ToString();
                    }
                    case false: {
                        foreach (var rapArrayEntry in rapArray.Entries) 
                            builder.Append(WriteParamXML(rapArrayEntry, indentation + 1, true)).Append('\n');
                        return builder.ToString();
                    }
                    
                }
            }
            case RapString rapString:
                return asArrVal 
                        ? builder.Append($"<item>{rapString.Value}</item>\n").ToString() 
                        : builder.Append(rapString.Value).ToString();
            case RapInteger rapInteger:
                return asArrVal 
                        ? builder.Append($"<item>{rapInteger.Value}</item>\n").ToString() 
                        : builder.Append(rapInteger.Value).ToString();
            case RapFloat rapFloat: 
                return asArrVal 
                        ? builder.Append($"<item>{rapFloat.Value}</item>\n").ToString() 
                        : builder.Append(rapFloat.Value).ToString();
            default: throw new NotSupportedException(token.GetType().Name);
        }

        return builder.ToString();
    }

}