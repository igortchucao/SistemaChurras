namespace Domain.Events
{
    public class InviteWasDeclined : IEvent
    {
        public string InviteId { get; set; }
    }
}
