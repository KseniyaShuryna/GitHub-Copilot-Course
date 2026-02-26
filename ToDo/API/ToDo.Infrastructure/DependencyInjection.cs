using ToDo.Infrastructure.DbContext;
using ToDo.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default");

            services.AddDbContext<ToDoDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<ITaskRepository, TaskRepository>();

            return services;
        }
    }
}
