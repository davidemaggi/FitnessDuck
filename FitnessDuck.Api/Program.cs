using System.Diagnostics;
using System.Globalization;
using System.Text;
using FitnessDuck.Core.Services.Implementations;
using FitnessData.Core.Services.Interfaces;
using FitnessDuck.Auth.Implementations;
using FitnessDuck.Auth.Interfaces;
using FitnessDuck.Data;
using FitnessDuck.Data.Repositories.Implementations;
using FitnessDuck.Data.Repositories.Interfaces;
using FitnessDuck.Exceptions;
using FitnessDuck.Mail.Implementations;
using FitnessDuck.Mail.Interfaces;
using FitnessDuck.Models;
using FitnessDuck.Models.Mappers;
using FitnessDuck.Notifications.Implementations;
using FitnessDuck.Notifications.Interfaces;
using FitnessDuck.TelegramBot.Implementations;
using FitnessDuck.TelegramBot.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FitnessDuck.Api;

public class Program
{
    public static void Main(string[] args)
    {
        
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // JWT Bearer scheme definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
                              "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                              "Example: \"Bearer abcdef12345\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // Require the bearer token for all endpoints or specific ones
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference 
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "Bearer",
                        Name = "Authorization",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            // Optional: Include XML comments if you use them for API docs
            // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // c.IncludeXmlComments(xmlPath);
        });
        builder.Services.AddControllers();
        
        
        builder.Services.AddSingleton<IFitnessDuckDbContextFactory, FitnessDuckDbContextFactory>();      
        
        builder.Services.AddScoped<FitnessDuckDbContext>(sp =>
        {
            var factory = sp.GetRequiredService<IFitnessDuckDbContextFactory>();
            return factory.CreateDbContext();
        });

        
        
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<ILessonRepository, LessonRepository>();
        builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ITokenRepository, TokenRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

        
        builder.Services.AddScoped<ILessonService, LessonService>();
        builder.Services.AddScoped<IScheduleService, ScheduleService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        builder.Services.AddScoped<INotificationOutboxService, NotificationOutboxService>();
        
        
        
        builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

        
        
        builder.Services.AddMapster();
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfig).Assembly);
        config.Default.MaxDepth(4);

        
        builder.Services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });
        
            
        builder.Logging.AddConsole(); 
        
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("TrainerOnly", policy => policy.RequireRole("Trainer"));

        });
        
        
        var telegramBotToken = builder.Configuration["Telegram:Token"];
        builder.Services.AddSingleton<ITelegramBotService>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TelegramBotService>>();
            var sp = serviceProvider.GetRequiredService<IServiceProvider>();
          
            var conf = serviceProvider.GetRequiredService<IConfiguration>();
            return new TelegramBotService(sp,logger, conf);
        });

        builder.Services.AddSingleton<IEmailSender, EmailSender>();
        
        builder.Services.AddHostedService<OutboxRunner>();
        
// In app middleware pipeline before other middlewares that handle requests
        
        var devCors = "_devCors";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: devCors,
                policy =>
                {
                    // Allow dev Blazor WASM app origin
                    policy.WithOrigins("http://localhost:5118") // Blazor dev port
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });


        
        
        

        var app = builder.Build();
        

        
        // Apply migrations on startup
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<FitnessDuckDbContext>();
            context.Database.Migrate();
        }

        
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            IdentityModelEventSource.ShowPII = true;
            
            app.UseCors(devCors);
            
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        

        app.MapControllers();

        
        
        
        var botService = app.Services.GetRequiredService<ITelegramBotService>();
        
        


        
      

        app.Run();
    }
}