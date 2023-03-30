using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

using SharedCore.Infrastructure.Middlewares;

using SharedCore.Model.Exceptions;

namespace SharedCore.Infrastructure.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void UseSharedInfrastructure(this WebApplication application)
        {
            UseInfrastructure(application);

            if (application.Environment.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }

            application.UseHttpsRedirection();

            application.UseAuthentication();
            application.UseAuthorization();

            application.MapControllers();

            application.UseHealthChecks("/health");
        }

        private static void UseInfrastructure(WebApplication application)
        {
            UseExceptionIfrastructure(application);
        }

        private static void UseExceptionIfrastructure(WebApplication application)
        {
            application.UseMiddleware<ExceptionMiddleware>();

            application.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAppException("Authentication has been failed.");
                }
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    throw new ForbiddenAppException("Authorization has been failed.");
                }
            });
        }
    }
}
