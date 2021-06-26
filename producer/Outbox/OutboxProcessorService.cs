using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using kafka_poc.Database;
using kafka_poc.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;

namespace kafka_poc.Outbox
{
    public class OutboxProcessorService : BackgroundService
    {
        readonly DatabaseConfig _databaseConfig;

        public OutboxProcessorService(DatabaseConfig databaseConfig) => _databaseConfig = databaseConfig;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SemaphoreManager.Lock(SemaphoreManager.Keys.Preferences);

                    using var db = new SqliteConnection(_databaseConfig.Name);
                    await db.OpenAsync();
                    var trans = await db.BeginTransactionAsync();

                    foreach (var item in await GetOutboxItems(db))
                    {
                        //Process item - logic to follow...
                        await ArchiveOutboxEntry(db, item);
                        await trans.CommitAsync();
                        Console.WriteLine(item.Data);
                    }

                    SemaphoreManager.Release(SemaphoreManager.Keys.Preferences);

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        static async Task ArchiveOutboxEntry(SqliteConnection db, OutboxModel item)
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

        static Task<IEnumerable<OutboxModel>> GetOutboxItems(SqliteConnection db) => db.QueryAsync<OutboxModel>("Select Id, TopicName, Data From Outbox;");
    }
}