using Domain.Entities;
using System;

namespace Domain.Events
{
    public class PersonHasBeenInvitedToBbq : IEvent
    {
        public string InviteId {  get; set; }

        public PersonHasBeenInvitedToBbq(string inviteId)
        {
            InviteId = inviteId;
        }
    }
}
