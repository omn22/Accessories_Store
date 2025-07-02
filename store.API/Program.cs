
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using store.Application.Interface.Services;
using store.Application.Services;
using Store.Application.Interface.Services;
using Store.Application.Services;
using Store.Domin.Enitity;
using Store.Infrastructure.Context;

namespace store.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            })
             .AddEntityFrameworkStores<StoreContext>()
             .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = builder.Configuration.GetValue<int>("AccountLock:TimesOfWrongPssword");
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("AccountLock:LockTime"));
            });
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, GenerateJWT>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<StoreContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
