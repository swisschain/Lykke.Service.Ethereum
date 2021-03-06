﻿using System;
using AzureStorage.Queue;
using Lykke.Service.EthereumCommon.Core.Domain;
using Lykke.Service.EthereumCommon.Core.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.EthereumCommon.AzureRepositories
{
    public class BalanceObservationTaskRepository : TaskRepositoryBase<BalanceObservationTask>, IBalanceObservationTaskRepository
    {
        private BalanceObservationTaskRepository(
            IQueueExt queue) 
            : base(queue)
        {
            
        }
        
        
        public static IBalanceObservationTaskRepository Create(
            IReloadingManager<string> connectionString)
        {
            var queue = AzureQueueExt.Create
            (
                connectionStringManager: connectionString,
                queueName: "balance-observation-tasks",
                maxExecutionTimeout: TimeSpan.FromDays(7)
            );
            
            return new BalanceObservationTaskRepository(queue);
        }
    }
}
