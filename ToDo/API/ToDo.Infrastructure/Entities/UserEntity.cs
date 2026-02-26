namespace ToDo.Infrastructure.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public ICollection<ToDoEntity> Tasks { get; set; } = new List<ToDoEntity>();
    }
}
