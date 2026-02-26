using ToDo.Infrastructure.Entities;

namespace ToDo.Infrastructure.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<ToDoEntity>> GetTasksByUserIdAsync(int userId);
        Task<ToDoEntity?> GetTaskByIdAsync(int id);
        Task AddTaskAsync(ToDoEntity task);
        Task UpdateTaskAsync(ToDoEntity task);
        Task DeleteTaskAsync(int id);
    }
}
