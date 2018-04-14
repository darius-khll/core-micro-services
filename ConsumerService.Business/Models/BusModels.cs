namespace ConsumerService.Business.Models
{
    public interface SubmitOrder
    {
        string OrderId { get; }
    }

    public interface OrderAccepted
    {
        string OrderId { get; }
    }


    public interface IPubSub
    {
        string Message { get; set; }
    }
    public class PubSub : IPubSub
    {
        public string Message { get; set; }
    }


    public interface IDataAdded : IPubSub
    {

    }
    public class DataAdded : IDataAdded
    {
        public string Message { get; set; }
    }
}
