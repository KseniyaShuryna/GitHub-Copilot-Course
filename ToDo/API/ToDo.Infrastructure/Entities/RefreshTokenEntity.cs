namespace ToDo.Infrastructure.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public int UserId { get; set; }
        public UserEntity? User { get; set; }
    }
}
