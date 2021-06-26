namespace kafka_poc.Models
{
    public class OutboxModel
    {
        public int Id { get; set; }
        public string TopicName { get; set; }
        public string Data { get; set; }
    }
}