namespace BisUtils.DZConfig.Models;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Param.Extensions;
using Param.Models.Statements;
using Param.Utils;

public interface IDzCfgMods : IParamConfigAbstraction<IParamClass>
{
    public ObservableCollection<IDzMod> Mods { get; }
}

public class DzCfgMods : ParamConfigAbstraction<IParamClass>, IDzCfgMods
{
    public ObservableCollection<IDzMod> Mods { get; private set; }

    public DzCfgMods(IParamClass ctx) : base(ctx)
    {
        Reset();
        Mods.CollectionChanged += ModsOnCollectionChanged;
    }

    private void ModsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
            {
                ParamContext.RemoveStatements(ParamContext.LocateBaseClasses());
                break;
            }
            case NotifyCollectionChangedAction.Add:
            {
                foreach (IDzMod item in e.NewItems)
                {
                    ParamContext.Statements.Add(item.ParamContext);
                }
                break;
            }
            case NotifyCollectionChangedAction.Remove:
            {
                ParamContext.RemoveStatements(e.OldItems.OfType<IDzMod>().Select(it => it.ParamContext));
                break;
            }
        }
    }

    protected void Reset() => Mods = new ObservableCollection<IDzMod>(ParamContext.LocateBaseClasses().Select(it => new DzMod(it)));
}

