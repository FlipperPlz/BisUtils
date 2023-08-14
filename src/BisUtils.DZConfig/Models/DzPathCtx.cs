namespace BisUtils.DZConfig.Models;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using RvConfig.Extensions;
using RvConfig.Models.Literals;
using RvConfig.Models.Statements;
using RvConfig.Models.Stubs;
using RvConfig.Utils;

public interface IDzPathCtx<T> : IParamConfigAbstraction<IParamClass> where T : ParamPathString
{
    public string? Value { get; set; }
    public ObservableCollection<T> Paths { get; }
}

public class DzPathCtx<T> : ParamConfigAbstraction<IParamClass>, IDzPathCtx<T> where T : ParamPathString, new()
{

    public string? Value { get; set; }
    public ObservableCollection<T> Paths { get; private set; }
    public IParamArray FilesArray { get; }

    public DzPathCtx(IParamClass ctx) : base(ctx)
    {
        FilesArray = ctx.LocateVariable<IParamArray>("files").VariableValue as IParamArray;
        Reset();
        Paths.CollectionChanged += PathsOnCollectionChanged;
    }

    private void PathsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
            {
                FilesArray.Value.Clear();
                break;
            }
            case NotifyCollectionChangedAction.Add:
            {
                foreach (ParamPathString item in e.NewItems)
                {
                    FilesArray.Value.Add(item.ParamString);
                }
                break;
            }
            case NotifyCollectionChangedAction.Remove:
            {
                foreach (ParamPathString item in e.OldItems)
                {
                    FilesArray.Value.Remove(item.ParamString);
                }
                break;
            }
        }
    }

    protected void Reset() => Paths = new ObservableCollection<T>(FilesArray.Value.OfType<IParamString>().Select(it => new T() { ParamString = it}));


}
