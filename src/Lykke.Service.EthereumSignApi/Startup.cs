﻿using System;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.EthereumCommon.Core;
using Lykke.Service.EthereumSignApi.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.EthereumSignApi
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {        
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.Logs = logs =>
                {
                    
#if ENABLE_SENSITIVE_LOGGING

                    logs.AzureTableName 
                        = $"{Constants.BlockchainId}SignApiLog";
                    
                    logs.AzureTableConnectionStringResolver =
                        settings => settings.SignApiService.Db.LogsConnString;
    
#else
                    
                    // We should not log anything in production environment for security reasons
                    logs.UseEmptyLogging();

#endif
                    
                };
                options.SwaggerOptions = new LykkeSwaggerOptions
                {
                    ApiTitle = $"{Constants.BlockchainName} Sign Api"
                };
            });
        }

        public void Configure(IApplicationBuilder app)
        {   
            app
                .UseLykkeConfiguration();
        }
    }
}
