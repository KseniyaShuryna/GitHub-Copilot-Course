namespace ToDo.Infrastructure.Entities
{
    public class ToDoEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsComplete { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public UserEntity? UserEntity { get; set; }
    }
}
