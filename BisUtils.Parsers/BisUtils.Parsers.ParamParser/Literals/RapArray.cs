using System.Collections;
using System.Text;
using BisUtils.Parsers.ParamParser.Factories;
using BisUtils.Parsers.ParamParser.Interfaces;

namespace BisUtils.Parsers.ParamParser.Literals; 

public class RapArray : IRapArrayEntry, IEnumerable<IRapArrayEntry>, IRapDeserializable<Generated.ParamLang.ParamParser.LiteralArrayContext> {
    public static readonly RapArray EmptyArray = new RapArray(new List<IRapArrayEntry>());
    public List<IRapArrayEntry> Entries { get; set; }

    public RapArray(List<IRapArrayEntry> entries) => Entries = entries;
    
    public IRapSerializable ReadParseTree(Generated.ParamLang.ParamParser.LiteralArrayContext ctx) {
        Entries = ctx.literalOrArray().Select(RapLiteralFactory.Create).ToList();
        return this;
    }
    
    public IEnumerator<IRapArrayEntry> GetEnumerator() => Entries.GetEnumerator(); 
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public string ToString(int indentation = char.MinValue) =>
        new StringBuilder(string.Join(string.Empty, Enumerable.Repeat("\t", indentation)))
            .Append('{')
            .Append(string.Join(',', Entries.Select(v => v.ToString())))
            .Append('}').ToString();

    internal RapArray() {}

    public static RapArray FromContext(Generated.ParamLang.ParamParser.LiteralArrayContext ctx) =>
        (RapArray) new RapArray().ReadParseTree(ctx);
}