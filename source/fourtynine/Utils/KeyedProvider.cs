using Microsoft.Extensions.Options;

namespace fourtynine;

public sealed class KeyedProviderConfiguration<TBaseType> where TBaseType : class
{
    public Dictionary<string, System.Type> Mappings { get; set; } = new();
}

public sealed class KeyedProviderConfigurationBuilder<TBaseType> where TBaseType : class
{
    private readonly KeyedProviderConfiguration<TBaseType> _configuration;
    private readonly IServiceCollection _services;

    public KeyedProviderConfigurationBuilder(KeyedProviderConfiguration<TBaseType> configuration, IServiceCollection services)
    {
        _configuration = configuration;
        _services = services;
    }

    public KeyedProviderConfigurationBuilder<TBaseType> Add<T>(string key, ServiceLifetime lifetime = ServiceLifetime.Scoped) where T : class, TBaseType
    {
        _configuration.Mappings.Add(key, typeof(T));
        _services.Add(new ServiceDescriptor(typeof(T), typeof(T), lifetime));
        _services.Add(new ServiceDescriptor(typeof(TBaseType), typeof(T), lifetime));
        return this;
    }
}

public static class KeyedProviderExtensions
{
    public static KeyedProviderConfigurationBuilder<TBaseType> AddKeyed<TBaseType>(
        this WebApplicationBuilder builder, Action<KeyedProviderConfigurationBuilder<TBaseType>>? configure = null)
        where TBaseType : class
    {
        var services = builder.Services; 
        if (services.All(s => s.ServiceType != typeof(IKeyedProvider<TBaseType>)))
            services.AddSingleton<IKeyedProvider<TBaseType>, KeyedProvider<TBaseType>>();

        var key = typeof(TBaseType).FullName + "_KeyedConfig";
        KeyedProviderConfiguration<TBaseType> config;
        var properties = builder.Host.Properties;
        if (properties.TryGetValue(key, out var config0))
        {
            config = (KeyedProviderConfiguration<TBaseType>) config0;
        }
        else
        {
            config = new KeyedProviderConfiguration<TBaseType>();
            properties.Add(key, config);
            
            services.AddSingleton(config);
        }
        
        var builder1 = new KeyedProviderConfigurationBuilder<TBaseType>(config, services);
        configure?.Invoke(builder1);
        return builder1;
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
    
    public KeyedProvider(KeyedProviderConfiguration<TBaseType> mappings, IServiceProvider serviceProvider)
    {
        _mappings = mappings;
        _serviceProvider = serviceProvider;
    }

    public TBaseType? Get(string schemeName)
    {
        if (!_mappings.Mappings.TryGetValue(schemeName, out var providerType))
            throw new ArgumentException($"No type found for name {schemeName}");

        return (TBaseType?) _serviceProvider.GetService(providerType);
    }
}