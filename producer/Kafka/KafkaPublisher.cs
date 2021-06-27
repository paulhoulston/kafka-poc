using System.Threading.Tasks;

namespace kafka_poc.Kafka
{
    public class KafkaPublisher : KafkaPublisher.IPublishEventsToKafka
    {
        public interface IPublishEventsToKafka
        {
            Task Publish(string topicName, string data);
        }

        public async Task Publish(string topicName, string data)
        {
            //code to follow....
            
            await Task.Delay(100);
        }

    }
}