using ToDo.Application.DTOs;
using ToDo.Infrastructure.Entities;

namespace ToDo.Application.Mapping
{
    public static class UserMappingExtensions
    {
        public static UserDto MapToDto(this UserEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new UserDto
            {
                Id = entity.Id,
                Email = entity.Email,
                Role = entity.Role
            };
        }

        public static UserEntity MapToEntity(this UserDto dto, string passwordHash)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new UserEntity
            {
                Id = dto.Id,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role
            };
        }
    }
}
