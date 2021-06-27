using System.Net;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace kafka_poc.Kafka
{
    public class KafkaPublisher : KafkaPublisher.IPublishEventsToKafka
    {
        readonly KafkaConfig _kafkaConfig;

        public interface IPublishEventsToKafka
        {
            Task PublishAsync(string topicName, string data);
        }

        public KafkaPublisher(KafkaConfig kafkaConfig)
        {
            _kafkaConfig = kafkaConfig;
        }

        public async Task PublishAsync(string topicName, string data)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.BootstrapServers,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = true
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                await producer.ProduceAsync(topicName, new Message<Null, string> { Value = data });
            }
        }
    }
}