

namespace BisUtils.Parsers.ParamParser.Statements;

public abstract class ParamStatement : IComparable<ParamStatement> {
    public abstract int CompareTo(ParamStatement? other);
    
}
