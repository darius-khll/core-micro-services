namespace ConsumerService.Configurations
{
    public class ConsumerOptions
    {
        public string MongoHost { get; set; }
        public string RabbitHost { get; set; }
        public string RabbitUser { get; set; }
        public string RabbitPassword { get; set; }
        public string PostgressHost { get; set; }
        public string PostgresDbName { get; set; }
    }
}
