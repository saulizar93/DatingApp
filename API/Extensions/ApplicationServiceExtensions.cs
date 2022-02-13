using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        //this keyword to extend the IServiceCollection
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // to track online users
            services.AddSingleton<PresenceTracker>();

            // feed CloudinarySettings class with settings in appsettings.json section CloudinarySettings
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            // AddTransient is limited as soon as the method is completed
            // AddScoped is limited to http request
            services.AddScoped<ITokenService, TokenService>();

            // Cloudinary service (services are singletons)
            services.AddScoped<IPhotoService, PhotoService>();

            // Replace all the repositories with UnitOfWork, which centralizes DataContext
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // services.AddScoped<ILikesRepository, LikesRepository>();

            // services.AddScoped<IMessageRepository, MessageRepository>();

            // update the lastActive property of the user
            services.AddScoped<LogUserActivity>();

            // add the service for our repository
            // services.AddScoped<IUserRepository, UserRepository>();

            // Service to map an object to another
            // this will find the CreateMaps inside AutoMapperProfiles
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            //we want to inject this to other parts of the application
            //create connection string for database
            //we create DefaultsConnections in appsettings.Development.json
            services.AddDbContext<DataContext>( options => {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
    }

                    // Whether the connection string came from the local development configuration file
                    // or from the environment variable from Heroku, use it to set up your DbContext.
                    options.UseNpgsql(connStr);

            });

            return services;
        }
    }
}