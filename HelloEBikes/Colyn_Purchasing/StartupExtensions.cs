using Colyn_Purchasing.BLL;
using Colyn_Purchasing.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colyn_Purchasing
{
    public static class StartupExtensions
    {
        public static void AddPurchasingDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<EbikePurchasingContext>(options);
            services.AddTransient<PurchasingService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<EbikePurchasingContext>();
                return new PurchasingService(context);
            });
        }
    }
}
