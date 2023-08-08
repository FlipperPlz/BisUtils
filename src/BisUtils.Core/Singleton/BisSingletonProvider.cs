namespace BisUtils.Core.Singleton;

public class BisSingletonProvider
{
    private static readonly Dictionary<Type, BisSingleton> Instances = new();


    public static TSingleton LocateInstance<TSingleton>() where TSingleton : BisSingleton, new()
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

}
