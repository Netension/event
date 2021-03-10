using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Event.Hosting.LightInject.Registers;
using Netension.Event.Hosting.RabbitMQ;
using Netension.Event.Sample.Enumerations;
using Netension.Extensions.Correlation;
using Netension.Extensions.Security;
using Serilog;

namespace Netension.Event.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseLightInject()
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    services.RegistrateCorrelation();
                })
                .UseEventing(builder =>
                {
                    builder.UseRabbitMQ(EventingEnumerations.RabbitMQ);

                    builder.RegistrateEventHandlers(register => register.RegistrateHandlerFromAssemblyOf<Startup>());

                    builder.RegistrateEventPublishers((register) =>
                    {
                        register.RegistrateRabbitMQPublisher(EventingEnumerations.Publishers.Publisher);
                    });

                    builder.RegistrateEventListeners((register) =>
                    {
                        register.RegistrateRabbitMQListener(EventingEnumerations.Listeners.Listener);
                    });

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
