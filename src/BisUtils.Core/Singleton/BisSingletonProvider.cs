namespace BisUtils.Core.Singleton;

public static class BisSingletonProvider
{
    private static readonly Dictionary<Type, IBisSingleton> Instances = new();


    public static TSingleton LocateInstance<TSingleton>() where TSingleton : IBisSingleton, new()
    {
        var type = typeof(TSingleton);
        if (Instances.TryGetValue(type, out var instance))
        {
            return (TSingleton)instance;
        }

        var created = new TSingleton();
        Instances.TryAdd(type, created);
        return created;
    }

    public static void AddInstance<TSingleton>(TSingleton singleton) where TSingleton : IBisSingleton, new()
    {
        var type = typeof(TSingleton);
        Instances.TryAdd(type, singleton);
    }

}
