using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class Person : AggregateRoot
    {
        public string Name { get; set; }
        public bool IsCoOwner { get; set; }
        public List<string> Invites { get; set; }
        public Person()
        {
            Invites = new List<string>();
        }

        public void When(PersonHasBeenCreated @event)
        {
            Id = @event.Id;
            Name = @event.Name;
            IsCoOwner = @event.IsCoOwner;
        }

        public void When(PersonHasBeenInvitedToBbq @event)
        {
            Invites.Add(@event.InviteId);
        }
        

        public object? TakeSnapshot()
        {
            return new
            {
                Id,
                Name,
                IsCoOwner,
                Invites 
            };
        }
    }
}
