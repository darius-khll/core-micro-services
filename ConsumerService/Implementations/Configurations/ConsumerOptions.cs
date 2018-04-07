namespace ConsumerService.Implementations.Configurations
{
    public class ConsumerOptions
    {
        public string MongoHost { get; set; }
        public string RabbitHost { get; set; }
        public string RabbitUser { get; set; }
        public string RabbitPassword { get; set; }
    }
}
