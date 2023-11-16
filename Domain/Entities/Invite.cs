using Domain.Entities.Enums;
using Domain.Events;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Invite : AggregateRoot
    {
        public string Bbq { get; set; }
        public DateTime Date { get; set; }
        public InviteStatus Status { get; set; }
        public string BbqId { get; set; }
        public string PersonId { get; set; }

        public void When(InviteSend @event)
        {
            Id = @event.Id;
            BbqId = @event.BbqId;
            PersonId = @event.PersonId;
            Bbq = $"{@event.Date} - {@event.Reason}";
            Status = InviteStatus.Pending;
            Date = @event.Date;
        }

        public void When(InviteWasAccepted @event)
        {
            Status = InviteStatus.Accepted;
        }

        public void When(InviteWasDeclined @event)
        {
            Status = InviteStatus.Declined;
        }

        public object? TakeSnapshot()
        {
            return new
            {
                Id,
                BbqId,
                PersonId,
                Bbq,
                Date,
                Status
            };
        }
    }
}
