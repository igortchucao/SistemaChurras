namespace Domain.Events
{
    public class InviteWasAccepted : IEvent
    {
        public string InviteId { get; set; }
    }
}
