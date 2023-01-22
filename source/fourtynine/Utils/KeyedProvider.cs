using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace fourtynine;

public sealed class KeyedProviderRegistration<TBaseType>
{
    public string Key { get; }
    public System.Type Type { get; }
    
    public KeyedProviderRegistration(string key, System.Type type)
    {
        Key = key;
        Type = type;
    }
}

public readonly record struct KeyedProviderBuilder<TBaseType> where TBaseType : class
{
    private readonly IServiceCollection _services;

    public KeyedProviderBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public KeyedProviderBuilder<TBaseType> Add<T>(string key, ServiceLifetime lifetime = ServiceLifetime.Scoped) where T : class, TBaseType
    {
        _services.AddSingleton(new KeyedProviderRegistration<TBaseType>(key, typeof(T)));
        _services.TryAdd(new ServiceDescriptor(typeof(T), typeof(T), lifetime));
        return this;
    }
}

public static class KeyedProviderExtensions
{
    public static KeyedProviderBuilder<TBaseType> AddKeyed<TBaseType>(
        this IServiceCollection services, Action<KeyedProviderBuilder<TBaseType>>? configure = null)
        where TBaseType : class
    {
        services.TryAddSingleton<IKeyedProvider<TBaseType>, KeyedProvider<TBaseType>>();
        
        var builder1 = new KeyedProviderBuilder<TBaseType>(services);
        configure?.Invoke(builder1);
        return builder1;
    }

    public static TBaseType? GetKeyedService<TBaseType>(this IServiceProvider serviceProvider, string key)
        where TBaseType : class
    {
        var factory = serviceProvider.GetKeyedProvider<TBaseType>();
        return factory.Get(key);
    }
    
    public static TBaseType GetRequiredKeyedService<TBaseType>(this IServiceProvider serviceProvider, string key)
        where TBaseType : class
    {
        var result = serviceProvider.GetKeyedService<TBaseType>(key);
        if (result is null)
            throw new InvalidOperationException($"No service of type {typeof(TBaseType).Name} with key {key} was found.");
        return result;
    }
    
    public static IKeyedProvider<TBaseType> GetKeyedProvider<TBaseType>(this IServiceProvider serviceProvider)
        where TBaseType : class
    {
        return serviceProvider.GetRequiredService<IKeyedProvider<TBaseType>>();
    }
}

public interface IKeyedProvider<out TBaseType> where TBaseType : class
{
    TBaseType? Get(string schemeName);
}

public sealed class KeyedProvider<TBaseType> : IKeyedProvider<TBaseType>
    where TBaseType : class
{
    private readonly Dictionary<string, System.Type> _mappings;
    private readonly IServiceProvider _serviceProvider;
    
    public KeyedProvider(IEnumerable<KeyedProviderRegistration<TBaseType>> registrations, IServiceProvider serviceProvider)
    {
        _mappings = registrations.ToDictionary(m => m.Key, m => m.Type);
        _serviceProvider = serviceProvider;
    }

    public TBaseType? Get(string schemeName)
    {
        if (!_mappings.TryGetValue(schemeName, out var providerType))
            return null;

        return (TBaseType?) _serviceProvider.GetRequiredService(providerType);
    }
}