using FSP.Api.Application.Common.Interfaces;
using FSP.Api.WebApiApi.Services;
using Microsoft.AspNetCore.Mvc;
using FSP.Api.WebApi.Configurations;
using System.Globalization;
using FSP.Api.Domain.Notifications;
using FSP.Api.Infrastructure.Data.DbContexts;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        var cultureInfo = new CultureInfo("pt-BR");
        cultureInfo.NumberFormat.CurrencySymbol = "R$";

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        services.AddScoped<IUser, CurrentUser>();

        services.AddNotifications();

        services.AddHttpContextAccessor();

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddSwaggerConfiguration();
        services.AddJwtAuthentication(configuration);

        return services;
    }
    private static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
        services.AddScoped<INotificationHandler<DomainSuccesNotification>, DomainSuccesNotificationHandler>();

        return services;
    }
}
