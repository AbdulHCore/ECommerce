namespace EventBus.Messages.Events
{
    public class BaseIntegrationEvent
    {
        public BaseIntegrationEvent()
        {
            CorrelationId = Guid.NewGuid().ToString();
            CreationDate = DateTime.UtcNow;
        }

        public BaseIntegrationEvent(string corelationId, DateTime creationDate)
        {
            CorrelationId= corelationId;
            CreationDate = creationDate;
        }

        public string CorrelationId { get; set; }
        public DateTime CreationDate { get; private set; }
        
    }
}
