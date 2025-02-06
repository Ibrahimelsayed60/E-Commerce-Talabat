
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Service;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services
            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            
            builder.Services.AddSwaggerServices();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            
            builder.Services.AddApplicationServices();

            builder.Services.AddIdentityServices(builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });

            #endregion

            var app = builder.Build();

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _dbContext = services.GetRequiredService<StoreContext>();

            var _identityDbContext = services.GetRequiredService<AppIdentityDbContext>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _dbContext.Database.MigrateAsync();

                await StoreContextSeed.SeedAsync(_dbContext);

                await _identityDbContext.Database.MigrateAsync();

                var _userManager = services.GetRequiredService<UserManager<AppUser>>();

                await AppIdentityDbContextSeed.SeedUsersAsync(_userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error has been occured during apply the migration");

            }

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("MyPolicy");

            app.MapControllers();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Run();
        }
    }
}
