//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.10.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/ryannkelly/Desktop/BisUtils/BisUtils.Generated/BisUtils.Generated.ParamLang/ParamParser.g4 by ANTLR 4.10.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace BisUtils.Generated.ParamLang;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="ParamParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.10.1")]
[System.CLSCompliant(false)]
public interface IParamParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.computationalStart"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComputationalStart([NotNull] ParamParser.ComputationalStartContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.enumDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnumDeclaration([NotNull] ParamParser.EnumDeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.enumValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEnumValue([NotNull] ParamParser.EnumValueContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatement([NotNull] ParamParser.StatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.arrayAppension"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrayAppension([NotNull] ParamParser.ArrayAppensionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.arrayTruncation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrayTruncation([NotNull] ParamParser.ArrayTruncationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.deleteStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDeleteStatement([NotNull] ParamParser.DeleteStatementContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.externalClassDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExternalClassDeclaration([NotNull] ParamParser.ExternalClassDeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.classDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitClassDeclaration([NotNull] ParamParser.ClassDeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.arrayDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrayDeclaration([NotNull] ParamParser.ArrayDeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.tokenDeclaration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTokenDeclaration([NotNull] ParamParser.TokenDeclarationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.literalArray"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteralArray([NotNull] ParamParser.LiteralArrayContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.literalString"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteralString([NotNull] ParamParser.LiteralStringContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.literalInteger"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteralInteger([NotNull] ParamParser.LiteralIntegerContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.literalFloat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteralFloat([NotNull] ParamParser.LiteralFloatContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.literalOrArray"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteralOrArray([NotNull] ParamParser.LiteralOrArrayContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLiteral([NotNull] ParamParser.LiteralContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.arrayName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitArrayName([NotNull] ParamParser.ArrayNameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="ParamParser.identifier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitIdentifier([NotNull] ParamParser.IdentifierContext context);
}
