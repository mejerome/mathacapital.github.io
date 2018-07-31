using System;
using MathaCapital.Areas.Identity.Data;
using MathaCapital.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(MathaCapital.Areas.Identity.IdentityHostingStartup))]
namespace MathaCapital.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MathaCapitalContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MathaCapitalContextConnection")));

                services.AddDefaultIdentity<MathaCapitalUser>()
                    .AddEntityFrameworkStores<MathaCapitalContext>();
            });
        }
    }
}