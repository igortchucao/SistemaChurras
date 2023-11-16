using System;
using Domain.Entities;

namespace Domain.Repositories
{
    internal class InviteRepository : StreamRepository<Invite>, IInviteRepository
    {
        public InviteRepository(IEventStore<Invite> eventStore) : base(eventStore) { }
    }
}
