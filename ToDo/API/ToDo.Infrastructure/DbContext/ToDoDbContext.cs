using Microsoft.EntityFrameworkCore;
using ToDo.Infrastructure.Entities;

namespace ToDo.Infrastructure.DbContext
{
    public class ToDoDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options) { }

        public DbSet<ToDoEntity> Tasks { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasMany(u => u.Tasks)
                      .WithOne(t => t.UserEntity)
                      .HasForeignKey(t => t.UserId);
            });

            modelBuilder.Entity<ToDoEntity>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasOne(i => i.UserEntity)
                    .WithMany(u => u.Tasks)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshTokenEntity>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
