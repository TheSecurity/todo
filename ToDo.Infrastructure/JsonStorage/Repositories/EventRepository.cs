using System.Threading.Tasks;
using ToDo.Application.Repositories;
using ToDo.Domain.Events;

namespace ToDo.Infrastructure.JsonStorage.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ToDoContext _context;

        public EventRepository(ToDoContext context)
        {
            _context = context;
        }

        public Task Add(ICalendarEvent cEvent)
        {
            _context.CalendarEvents.Add((CalendarEvent)cEvent);
            _context.Serialize();
            return Task.CompletedTask;
        }
    }
}
