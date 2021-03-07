using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Netension.Event.Hosting.LightInject.Registers;
using Netension.Event.Hosting.RabbitMQ;
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
                .UseEventing(builder =>
                {
                    builder.UseRabbitMQ((options, configuration) => { configuration.GetSection("RabbitMQ").Bind(options); options.Password = "guest".Encrypt(); });

                    builder.RegistrateEventHandlers(register => register.RegistrateHandlerFromAssemblyOf<Startup>());

                    builder.RegistrateEventPublishers((register) =>
                    {
                        register.RegistrateRabbitMQPublisher("rabbitmq", "RabbitMQ", (@event) => true, (options, configuration) => configuration.GetSection("RabbitMQ:Publish").Bind(options));
                    });

                    builder.RegistrateEventListeners((register) =>
                    {
                        register.RegistrateRabbitMQListener("rabbitmq", "RabbitMQ", (options, configuration) => configuration.GetSection("RabbitMQ:Listen").Bind(options));
                    });

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
