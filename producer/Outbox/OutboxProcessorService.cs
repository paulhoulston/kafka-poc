using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Database;
using kafka_poc.Models;
using Microsoft.Extensions.Hosting;

namespace kafka_poc.Outbox
{
    public class OutboxProcessorService : BackgroundService
    {
        const int THREAD_DELAY_MS = 1000;
        const int THREAD_DELAY_MS_FOR_EXCEPTION = 5000;
        readonly DatabaseWrapper.IAbstractAwayTheDatabase _dbAbstractor;

        public OutboxProcessorService(DatabaseWrapper.IAbstractAwayTheDatabase dbAbstractor) => _dbAbstractor = dbAbstractor;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _dbAbstractor.ExecuteAsync(Go);
                    await Task.Delay(THREAD_DELAY_MS, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await Task.Delay(THREAD_DELAY_MS_FOR_EXCEPTION, stoppingToken);
                }
            }
        }

        async Task Go(IDbConnection db)
        {
            foreach (var item in await GetOutboxItems(db))
            {
                //Process item - logic to follow...
                await ArchiveOutboxEntry(db, item);
            }
        }

        static async Task ArchiveOutboxEntry(IDbConnection db, OutboxModel item)
        {
            await db.ExecuteAsync(
                @"Insert Into OutboxArchive(TopicName, Data)
                    Select TopicName, Data
                    From Outbox
                    Where Id = @Id", new
                {
                    Id = item.Id,
                    Created = DateTime.UtcNow
                });

            await db.ExecuteAsync(@"Delete From Outbox Where Id = @Id", new { item.Id });
        }

        static Task<IEnumerable<OutboxModel>> GetOutboxItems(IDbConnection db) => db.QueryAsync<OutboxModel>("Select Id, TopicName, Data From Outbox;");
    }
}