using System.Reflection;
using System.Text;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using SharedCore.Infrastructure.Behaviours;
using SharedCore.Infrastructure.Loggers;

using SharedCore.Model.Options;

namespace SharedCore.Infrastructure.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void AddSharedInfrastructure(this WebApplicationBuilder builder, Assembly? domainAssembly = null)
        {
            builder.Logging.ClearProviders();
            builder.Logging.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, JsonLoggerProvider>());

            AddInfrastructure(builder.Services, builder.Configuration, domainAssembly);

            builder.Services.AddAuthorization();

            builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHealthChecks();
        }

        private static void AddInfrastructure(IServiceCollection services, IConfiguration configuration, Assembly? domainAssembly)
        {
            AddAuthentication(services, configuration);

            AddSwagger(services);

            AddDomainInfrastructure(services, domainAssembly);
        }

        private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection(JwtOptions.Jwt);
            services.Configure<JwtOptions>(jwtSection);

            var jwtOptions = jwtSection.Get<JwtOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = jwtOptions?.Audience,
                        ValidIssuer = jwtOptions?.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.SecurityKey!))
                    };
                });
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                                  "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                                  "Example: \"Bearer 1safsfsdfdfd\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme { Reference = new OpenApiReference {  Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                        new string[] {}
                    }
                });
            });
        }

        private static void AddDomainInfrastructure(IServiceCollection services, Assembly? domainAssembly)
        {
            if (domainAssembly == null)
            {
                Console.WriteLine("domainAssembly == null");

                return;
            }

            services.AddValidatorsFromAssembly(domainAssembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddAutoMapper(domainAssembly);

            services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(domainAssembly));
        }
    }
}
