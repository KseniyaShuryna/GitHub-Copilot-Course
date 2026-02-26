using Microsoft.Extensions.DependencyInjection;
using ToDo.Application.Interfaces;
using ToDo.Application.Services;

namespace ToDo.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IToDoService, ToDoService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
