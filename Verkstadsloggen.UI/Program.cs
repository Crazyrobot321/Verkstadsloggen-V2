using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Verkstadsloggen.Application.Interface.Repository;
using Verkstadsloggen.Application.Interface.Service;
using Verkstadsloggen.Application.Services;
using Verkstadsloggen.Infrastructure.Data;
using Verkstadsloggen.Infrastructure.Identity;
using Verkstadsloggen.Infrastructure.Repository;

namespace Verkstadsloggen.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            //Register the repositories with dependency injection
            builder.Services.AddScoped<IMechanicRepository, MechanicRepository>();
            builder.Services.AddScoped<IJobRepository, JobRepository>();
            builder.Services.AddScoped<IJobService, JobService>();
            builder.Services.AddScoped<IMechanicService, MechanicService>();

            builder.Services.AddScoped<ITimeLogRepository, TimeLogRepository>();
            builder.Services.AddScoped<ITimeLogService, TimeLogService>();

            builder.Services.AddScoped<IPersonRepository, PersonRepository>();
            builder.Services.AddScoped<IPersonService, PersonService>();

            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
                    options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<MyDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();


            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            await DataSeeder.SeedIdentityAsync(app.Services);
            if (app.Environment.IsDevelopment())
            {
                await DataSeeder.SeedTestDataAsync(app.Services);

            }
            app.Run();
        }
    }
}
