using Microsoft.Extensions.Options;

namespace fourtynine;

public sealed class KeyedProviderConfiguration<TBaseType> where TBaseType : class
{
    public Dictionary<string, System.Type> Mappings { get; set; } = new();
}

public sealed class KeyedProviderConfigurationBuilder<TBaseType> where TBaseType : class
{
    public List<(string, System.Type, ServiceLifetime)> Mappings { get; } = new();
    public KeyedProviderConfigurationBuilder<TBaseType> Add<T>(string key, ServiceLifetime lifetime = ServiceLifetime.Scoped) where T : class, TBaseType
    {
        Mappings.Add((key, typeof(T), lifetime));
        return this;
    }
}

public static class KeyedProviderExtensions
{
    public static void AddKeyed<TBaseType>(
        this IServiceCollection services, Action<KeyedProviderConfigurationBuilder<TBaseType>> configure)
        where TBaseType : class
    {
        if (services.All(s => s.ServiceType != typeof(IKeyedProvider<TBaseType>)))
            services.AddSingleton<IKeyedProvider<TBaseType>, KeyedProvider<TBaseType>>();

        var builder = new KeyedProviderConfigurationBuilder<TBaseType>();
        configure(builder);

        foreach (var (_, type, lifetime) in builder.Mappings)
        {
            services.Add(new ServiceDescriptor(type, type, lifetime));
            services.Add(new ServiceDescriptor(typeof(TBaseType), type, lifetime));
        }
        
        var optionsBuilder = services.AddOptions<KeyedProviderConfiguration<TBaseType>>();
        optionsBuilder.Configure(o =>
        {
            foreach (var (key, type, _) in builder.Mappings)
                o.Mappings.Add(key, type);
        });
    }

    public static TBaseType? GetKeyedService<TBaseType>(this IServiceProvider serviceProvider, string key) where TBaseType : class
    {
        var factory = serviceProvider.GetKeyedProvider<TBaseType>();
        return factory.Get(key);
    }
    
    public static TBaseType GetRequiredKeyedService<TBaseType>(this IServiceProvider serviceProvider, string key) where TBaseType : class
    {
        var result = serviceProvider.GetKeyedService<TBaseType>(key);
        if (result is null)
            throw new InvalidOperationException($"No service of type {typeof(TBaseType).Name} with key {key} was found.");
        return result;
    }
    
    public static IKeyedProvider<TBaseType> GetKeyedProvider<TBaseType>(this IServiceProvider serviceProvider) where TBaseType : class
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
    private readonly KeyedProviderConfiguration<TBaseType> _mappings;
    private readonly IServiceProvider _serviceProvider;
    
    public KeyedProvider(IOptions<KeyedProviderConfiguration<TBaseType>> mappings, IServiceProvider serviceProvider)
    {
        _mappings = mappings.Value;
        _serviceProvider = serviceProvider;
    }

    public TBaseType? Get(string schemeName)
    {
        if (!_mappings.Mappings.TryGetValue(schemeName, out var providerType))
            throw new ArgumentException($"No type found for name {schemeName}");

        return (TBaseType?) _serviceProvider.GetService(providerType);
    }
}