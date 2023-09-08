namespace BisUtils.RvConfig.Models.Stubs.Holders;

using Extensions;
using Statements;

public interface IParamStatementHolder : IParamElement
{
    IParamStatementHolder ParentClass { get; set; }
    List<IParamStatement> Statements { get; set; }

    public static IParamClass? operator >>(IParamStatementHolder clazz, string clazzName) =>
        clazz.LocateBaseClass(clazzName);
}
