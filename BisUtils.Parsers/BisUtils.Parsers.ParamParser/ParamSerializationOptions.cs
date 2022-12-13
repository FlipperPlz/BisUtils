using BisUtils.Core;
using BisUtils.Core.Serialization.Options;

namespace BisUtils.Parsers.ParamParser; 

public enum ParamLanguage {
    CPP,
    XML
}

public class RapSerializationOptions : BisSerializationOptions {
    public static readonly RapSerializationOptions DefaultOptions = new RapSerializationOptions();

    public bool OrganizeEntries { get; set; } = true;
    public ParamLanguage Language { get; set; } = ParamLanguage.CPP;
}

public class RapDeserializationOptions : BisDeserializationOptions {
    public static readonly RapSerializationOptions DefaultOptions = new RapSerializationOptions();
    
    public ParamLanguage Language { get; set; } = ParamLanguage.CPP;
}