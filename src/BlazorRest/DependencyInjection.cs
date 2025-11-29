using BlazorRest.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using BlazorRest.BlazorRest.Interceptors;

namespace BlazorRest
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBlazorRest(this IServiceCollection services, Action<BlazorRestOptions>? options=default)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var serviceOptions = new BlazorRestOptions();
            options?.Invoke(serviceOptions);
            ConfigOptions(services, serviceOptions);
            services.AddScoped<IBlazorRest, BlazorRestClient>();
            return services;
        }


        private static IServiceCollection ConfigOptions(IServiceCollection services,BlazorRestOptions serviceOptions)
        {
             services.AddSingleton(Options.Create(serviceOptions));

            if (serviceOptions.JwtTokenService is not null)
            {
                services.AddScoped(typeof(IJwtService), serviceOptions.JwtTokenService);
            }

            if (serviceOptions.RequestInterceptor is not null)
            {
                services.AddScoped(typeof(IRequestInterceptor), serviceOptions.RequestInterceptor);
            }
            else
            {
                services.AddScoped<IRequestInterceptor, DefaultRequestInterceptor>();
            }

            if (serviceOptions.ResponseInterceptor is not null)
            {
                services.AddScoped(typeof(IResponseInterceptor), serviceOptions.ResponseInterceptor);
            }
            else
            {
                services.AddScoped<IResponseInterceptor, DefaultResponseInterceptor>();
            }

            if (serviceOptions.ErrorInterceptor is not null)
            {
                services.AddScoped(typeof(IErrorInterceptor), serviceOptions.ErrorInterceptor);
            }
            else
            {
                services.AddScoped<IErrorInterceptor, DefaultErrorInterceptor>();
            }

            if (serviceOptions.BaseUri is null)
            {
                services.AddScoped(sp => new HttpClient());
            }
            else
            {
                services.AddScoped(sp => new HttpClient { BaseAddress = serviceOptions.BaseUri });
            }

         
            return services;
        }
    }
}
