using ToDo.Application.DTOs;
using ToDo.Infrastructure.Entities;

namespace ToDo.Application.Mapping
{
    public static class ToDoMappingExtensions
    {
        public static ToDoDto MapToDto(this ToDoEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new ToDoDto
            {
                Id = entity.Id,
                Title = entity.Title,
                IsComplete = entity.IsComplete,
                CreatedAt = entity.CreatedAt
            };
        }

        public static ToDoEntity MapToEntity(this ToDoDto dto, int userId)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new ToDoEntity
            {
                Id = dto.Id,
                Title = dto.Title,
                IsComplete = dto.IsComplete,
                CreatedAt = dto.CreatedAt,
                UserId = userId
            };
        }
    }
}
