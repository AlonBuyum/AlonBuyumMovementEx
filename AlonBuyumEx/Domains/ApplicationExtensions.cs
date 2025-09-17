using System;
using System.Diagnostics;
using System.Text;

using AlonBuyumEx.DAL;
using AlonBuyumEx.Domains.DataDomain;
using AlonBuyumEx.Services;

using Microsoft.IdentityModel.Tokens;

using StackExchange.Redis;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using AlonBuyumEx.Domains.AuthDomain;

namespace AlonBuyumEx.Domains
{
    public static class ApplicationExtensions
    {
        public static void MapEndPoints(this WebApplication app)
        {
            // Add other endpoint mapping here when needed
            DataEndpoints.MapDataEndPoints(app);
            AuthEndpoints.MapAuthEndPoints(app);
        }

        /// <summary>
        /// Adds application services we created to IServiceCollection, so it could be used in Program.cs
        /// </summary>
        /// <param name="configuration">Send builder.Configuration here</param>
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {// Add other services here when needed
            
            services.AddAppCors(configuration);
            // file storage service
            services.AddSingleton<IFileStorageService, FileStorageService>();
            // memory cache service
            var redisConn = configuration.GetValue<string>("Redis:Connection");
            if (!string.IsNullOrEmpty(redisConn))
            {
                try
                {
                    // try to connect and add redis
                    var redis = ConnectionMultiplexer.Connect(redisConn);
                    services.AddSingleton<IConnectionMultiplexer>(redis);
                    services.AddSingleton<ICacheProvider, RedisCacheProvider>();
                }
                catch (Exception ex)
                {
                    // if redis fails fall back to memory cache
                    Trace.WriteLine($"Redis connection failed:\n{ex}");
                    services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();
                }
            }
            else
            {
                // if no redis connection string fall back to memory cache
                services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();
            }
            services.AddMemoryCache();

            // add Sql DB service
            services.AddScoped<IDataRepository, DataRepository>();

            // add DB factory
            // add other repository creators for other DBs here when needed
            services.AddScoped<DataRepositoryCreator>(); 

            //add main logic service with decorator
            services.AddScoped< DataService>(); // first add DataSerice itself
            // then add IDataService. in the lambda function -
            // use the provider param to get another service from all the services that were added previously.
            // then we can add another service that also implements IDataService - that service can then receive other services as params in its constructor.
            services.AddScoped<IDataService>(provider =>
                new DataServiceDecorator(provider.GetRequiredService<DataService>(),
                    provider.GetRequiredService<ILogger<DataServiceDecorator>>())
            );

            // add authentication and authorization
            services.AddJwtAuth(configuration);

            // add Swagger with auth enabled
            services.AddSwaggerWithAuth();

            // add JWT auth service
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
        private static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddCors(options => options.AddPolicy("AllowedPolicy", policy =>
            {
                var origins = configuration.GetSection("AllowedOrigins").Get<string[]>();
                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }));
        }

        private static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration.GetValue<string>("Jwt:Secret");
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
             {
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true ,
                     ValidIssuer = configuration["Jwt:Issuer"],
                     ValidAudience = configuration["Jwt:Audience"],
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = signingKey,
                     ClockSkew = TimeSpan.Zero
                 };
             });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("User", "Admin"));
            });

            return services;
        }

        private static IServiceCollection AddSwaggerWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Add JWT Bearer",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new  OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                        []
                    }

                });
            });
            return services;
        }
    }
}
