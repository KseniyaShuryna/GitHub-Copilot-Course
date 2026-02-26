using ToDo.Application.DTOs;

namespace ToDo.Application.Interfaces
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoDto>> GetToDosByUserIdAsync();
        Task<ToDoDto> GetToDoByIdAsync(int id);
        Task<ToDoDto> AddToDoAsync(ToDoDto toDoDto);
        Task<ToDoDto> ToggleToDoAsync(int id);
        Task DeleteToDoAsync(int id);
    }
}
