using ToDo.Application.Exceptions;
using ToDo.Application.DTOs;
using ToDo.Application.Interfaces;
using ToDo.Infrastructure.Repositories;
using ToDo.Application.Mapping;

namespace ToDo.Application.Services
{
    /// <summary>
    /// Service for managing ToDo items, including CRUD operations and validation.
    /// </summary>
    public class ToDoService : IToDoService
    {
        private const int MinTitleLength = 3;
        private const int MaxTitleLength = 100;
        private const int MaxToDosPerUser = 1000;

        private readonly ITaskRepository _taskRepository;
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoService"/> class.
        /// </summary>
        /// <param name="taskRepository">Repository for ToDo tasks.</param>
        /// <param name="currentUserService">Service for retrieving the current user.</param>
        public ToDoService(ITaskRepository taskRepository, ICurrentUserService currentUserService)
        {
            _taskRepository = taskRepository;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Retrieves all ToDo items for a specific user.
        /// </summary>
        /// <returns>A collection of ToDo items.</returns>
        public async Task<IEnumerable<ToDoDto>> GetToDosByUserIdAsync()
        {
            var userId = _currentUserService.GetUserId();
            var todos = await _taskRepository.GetTasksByUserIdAsync(userId);

            return todos.Select(t => t.MapToDto());
        }

        /// <summary>
        /// Retrieves a ToDo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        /// <returns>The ToDo item.</returns>
        /// <exception cref="NotFoundException">Thrown if the ToDo item is not found.</exception>
        public async Task<ToDoDto> GetToDoByIdAsync(int id)
        {
            var toDoEntity = await _taskRepository.GetTaskByIdAsync(id);

            if (toDoEntity == null)
                throw new NotFoundException($"ToDo item with id {id} not found.");

            if (toDoEntity.UserId != _currentUserService.GetUserId())
                throw new UnauthorizedException("You do not have permission to modify this ToDo item.");

            return toDoEntity.MapToDto();
        }

        /// <summary>
        /// Adds a new ToDo item for the current user after validating the input.
        /// </summary>
        /// <param name="toDoDto">The ToDo item data transfer object.</param>
        /// <returns>The created ToDo item.</returns>
        /// <exception cref="ValidationException">Thrown if validation fails.</exception>
        public async Task<ToDoDto> AddToDoAsync(ToDoDto toDoDto)
        {
            var userId = _currentUserService.GetUserId();

            // Title required
            if (string.IsNullOrWhiteSpace(toDoDto.Title))
                throw new ValidationException("Title is required.");

            // Title length
            if (toDoDto.Title.Length < MinTitleLength || toDoDto.Title.Length > MaxTitleLength)
                throw new ValidationException($"Title must be between {MinTitleLength} and {MaxTitleLength} characters.");

            // Title format: only printable chars (letters, digits, whitespace, punctuation)
            if (!toDoDto.Title.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)))
                throw new ValidationException("Title contains invalid characters.");

            // Optional: Check for duplicate title per user
            var existing = await _taskRepository.GetTasksByUserIdAsync(userId);

            if (existing.Any(t => t.Title.Equals(toDoDto.Title, StringComparison.OrdinalIgnoreCase)))
                throw new ValidationException("A ToDo with this title already exists.");

            // Optional: Max ToDos per user
            if (existing.Count() >= MaxToDosPerUser)
                throw new ValidationException($"You have reached the maximum number of ToDos allowed ({MaxToDosPerUser}).");

            var todo = toDoDto.MapToEntity(userId);
            todo.IsComplete = false;
            todo.CreatedAt = DateTime.UtcNow;

            await _taskRepository.AddTaskAsync(todo);

            return todo.MapToDto();
        }

        /// <summary>

        /// Toggles the completion status of a ToDo item and returns the updated item.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        /// <returns>The updated ToDo item.</returns>
        /// <exception cref="NotFoundException">Thrown if the ToDo item is not found.</exception>
        public async Task<ToDoDto> ToggleToDoAsync(int id)
        {
            var toDoEntity = await _taskRepository.GetTaskByIdAsync(id);

            if (toDoEntity == null)
                throw new NotFoundException($"ToDo item with id {id} not found.");
            if (toDoEntity.UserId != _currentUserService.GetUserId())
                throw new UnauthorizedException("You do not have permission to modify this ToDo item.");

            toDoEntity.IsComplete = !toDoEntity.IsComplete;
            await _taskRepository.UpdateTaskAsync(toDoEntity);

            return toDoEntity.MapToDto();
        }

        /// <summary>
        /// Deletes a ToDo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the ToDo item.</param>
        public async Task DeleteToDoAsync(int id)
        {
            var toDoEntity = await _taskRepository.GetTaskByIdAsync(id);

            if (toDoEntity == null)
                throw new NotFoundException($"ToDo item with id {id} not found.");

            if (toDoEntity.UserId != _currentUserService.GetUserId())
                throw new UnauthorizedException("You are not authorized to delete this ToDo item.");

            await _taskRepository.DeleteTaskAsync(id);
        }
    }
}
