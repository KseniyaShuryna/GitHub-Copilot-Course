using Microsoft.EntityFrameworkCore;
using ToDo.Infrastructure.Entities;
using ToDo.Infrastructure.DbContext;

namespace ToDo.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ToDoDbContext _context;

        public TaskRepository(ToDoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ToDoEntity>> GetTasksByUserIdAsync(int userId)
        {
            return await _context.Tasks.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<ToDoEntity?> GetTaskByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task AddTaskAsync(ToDoEntity task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(ToDoEntity task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
