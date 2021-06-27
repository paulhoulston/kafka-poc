using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using kafka_poc.Database;
using kafka_poc.Kafka;
using kafka_poc.Models;
using Microsoft.Extensions.Hosting;

namespace kafka_poc.Outbox
{
    public class OutboxProcessorService : BackgroundService
    {
        const int THREAD_DELAY_MS = 1000;
        const int THREAD_DELAY_MS_FOR_EXCEPTION = 5000;
        readonly DatabaseWrapper.IAbstractAwayTheDatabase _dbWrapper;
        readonly OutboxArchiver.IArchiveOutboxItems _outboxArchiver;
        readonly KafkaPublisher.IPublishEventsToKafka _kafkaPublisher;
        readonly OutboxLister.IGetAllOutboxItems _outboxLister;

        public OutboxProcessorService(
            DatabaseWrapper.IAbstractAwayTheDatabase dbWrapper,
            OutboxArchiver.IArchiveOutboxItems outboxArchiver,
            KafkaPublisher.IPublishEventsToKafka kafkaPublisher,
            OutboxLister.IGetAllOutboxItems outboxLister)
        {
            _dbWrapper = dbWrapper;
            _outboxArchiver = outboxArchiver;
            _kafkaPublisher = kafkaPublisher;
            _outboxLister = outboxLister;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _dbWrapper.ExecuteAsync(Go);
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
            var outboxItems = await _outboxLister.GetAllAsync(db);
            outboxItems.NullSafeForEach(async item =>
            {
                await PublishEventsToKafka(item);
                await _outboxArchiver.ArchiveOutboxEntryAsync(db, item);
            });
        }

        async Task PublishEventsToKafka(OutboxModel kafkaData)
        {
            await _kafkaPublisher.PublishAsync(kafkaData.TopicName, kafkaData.Data);
        }
    }
}