namespace R.Systems.Queue.WebApi;

public static class DependencyInjection
{
    public static void ConfigureWebApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.ConfigureSwagger();
        //services.ConfigureServiceBusListeners();
    }

    private static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }

    //private static void ConfigureServiceBusListeners(this IServiceCollection services)
    //{
    //    services.ConfigureServiceBusQueueListener<SitesBatchListener, GroupedSitesQueueOptions>(
    //        new ServiceBusProcessorOptions
    //        {
    //            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
    //            MaxConcurrentCalls = 1
    //        }
    //    );
    //}
}
