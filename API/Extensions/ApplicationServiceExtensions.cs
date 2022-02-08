using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
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
            // AddTransient is limited as soon as the method is completed
            // AddScoped is limited to http request
            services.AddScoped<ITokenService, TokenService>();

            // add the service for our repository
            services.AddScoped<IUserRepository, UserRepository>();

            // Service to map an object to another
            // this will find the CreateMaps inside AutoMapperProfiles
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            
            //we want to inject this to other parts of the application
            //create connection string for database
            //we create DefaultsConnections in appsettings.Development.json
            services.AddDbContext<DataContext>( options => {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}