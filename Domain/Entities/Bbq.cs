using System;
using System.Collections.Generic;
using Domain.Entities.Enums;
using Domain.Events;

namespace Domain.Entities
{
    public class Bbq : AggregateRoot
    {
        public string Reason { get; set; }
        public BbqStatus Status { get; set; }
        public DateTime Date { get; set; }
        public bool IsTrincasPaying { get; set; }
        public List<string> InvitationsAccepted { get; set; }

        public Bbq() 
        { 
            InvitationsAccepted = new List<string>();
        }

        public void When(BbqCreated @event)
        {
            Id = @event.Id.ToString();
            Date = @event.Date;
            Reason = @event.Reason;
            Status = BbqStatus.New;
        }

        public void When(BbqStatusUpdated @event)
        {
            if (@event.GonnaHappen)
                Status = BbqStatus.PendingConfirmations;
            else 
                Status = BbqStatus.ItsNotGonnaHappen;

            IsTrincasPaying = @event.TrincaWillPay;
        }

        public void When(InviteWasAccepted @event)
        {
            InvitationsAccepted.Add(@event.InviteId);

            if (Status != BbqStatus.ItsNotGonnaHappen)
            {
                if (InvitationsAccepted.Count >= 7)
                    Status = BbqStatus.Confirmed;
            }
        }    
        
        public void When(InviteWasDeclined @event)
        {
            InvitationsAccepted.Remove(@event.InviteId);

            if(Status != BbqStatus.ItsNotGonnaHappen)
            {
                if (InvitationsAccepted.Count < 7)
                    Status = BbqStatus.PendingConfirmations;
            }
        }

        public object TakeSnapshot()
        {
            return new
            {
                Id,
                Date,
                IsTrincasPaying,
                Status = Status.ToString()
            };
        }
    }
}
