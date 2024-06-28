namespace paperlessDocumentProcessor;

public static class ConfigHelper
{
    public static void AddConfig<T>(this WebApplicationBuilder builder) where T : class
    {
        var config = builder.Configuration.GetSection(typeof(T).Name);
        builder.Services.Configure<T>(config, o => o.BindNonPublicProperties = true);
    }

    public static T GetConfig<T>(this WebApplicationBuilder builder) where T : class
    {
        var configurationSection = builder.Configuration.GetSection(typeof(T).Name);
        return configurationSection.Get<T>() ?? throw new Exception("Configuration not found");
    }
}