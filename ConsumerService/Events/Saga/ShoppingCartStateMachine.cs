using Automatonymous;
using System;

namespace ConsumerService.Events.Saga
{
    public class ShoppingCartStateMachine : MassTransitStateMachine<ShoppingCart>
    {
        public ShoppingCartStateMachine()
        {
        }
    }

    public class ShoppingCart : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }
        public DateTime ReceivedDateTime { get; set; }
        public DateTime RegisteredDateTime { get; set; }
        public string PickupName { get; set; }
    }
}
