using AppSecurity.BLL;
using AppSecurity.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSecurity
{
    public static class StartupExtensions
    {
        public static void AddAppSecurityDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<AppSecurityDbContext>(options);
            services.AddTransient<SecurityService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<AppSecurityDbContext>();
                return new SecurityService(context);
            });
        }
    }
}
