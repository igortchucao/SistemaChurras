using System;

namespace Domain.Events
{
    public class InviteSend : IEvent
    {
        public string Id { get; }
        public DateTime BBQDate { get; }
        public DateTime Date { get; }
        public string Reason { get; }
        public string PersonId { get; }
        public string BbqId { get; }

        public InviteSend(string id, DateTime date, DateTime bbqDate, string reason, string personId, string bbqId)
        {
            Id = id;
            Date = date;
            BBQDate = bbqDate;
            Reason = reason;
            PersonId = personId;
            BbqId = bbqId;
        }
    }
}
