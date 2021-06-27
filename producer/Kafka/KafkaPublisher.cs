using System.Threading.Tasks;

namespace kafka_poc.Kafka
{
    public class KafkaPublisher : KafkaPublisher.IPublishEventsToKafka
    {
        public interface IPublishEventsToKafka
        {
            Task PublishAsync(string topicName, string data);
        }

        public async Task PublishAsync(string topicName, string data)
        {
            //code to follow....
            
            await Task.Delay(100);
        }

    }
}