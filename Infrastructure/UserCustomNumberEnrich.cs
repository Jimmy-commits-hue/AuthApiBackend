using Serilog.Core;
using Serilog.Events;
using Microsoft.AspNetCore.Http;

namespace AuthApi.Infrastructure
{

    public class UserCustomNumberEnrich : ILogEventEnricher
    {

        private readonly IHttpContextAccessor httpContextAccessor;

        public UserCustomNumberEnrich(IHttpContextAccessor httpContextAccessor)
        {

            this.httpContextAccessor = httpContextAccessor;

        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {

            var userCustomNumber = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userCustomNumber))
            {

                return;

            }

            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("UserCustomNumber", userCustomNumber));

        }

    }

}
