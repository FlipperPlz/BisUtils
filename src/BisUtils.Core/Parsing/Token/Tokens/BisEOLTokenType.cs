﻿namespace BisUtils.Core.Parsing.Token.Tokens;

using Typing;


public abstract class BisEOLTokenTypeCore : BisTokenTypeCore
{
    public abstract string TokenSetName { get; }
    public override string TokenRegex => "(\r?\n)";
    public override string TokenType => $"{TokenSetName}.EOL";
}

public class BisEOLTokenType : BisEOLTokenTypeCore
{
    public override string TokenSetName { get; }

    public BisEOLTokenType(string tokenSetName) => TokenSetName = tokenSetName;

}