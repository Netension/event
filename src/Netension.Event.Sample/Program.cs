using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Netension.Event.Hosting.LightInject.Registers;
using Netension.Event.Hosting.RabbitMQ;
<<<<<<< Updated upstream
=======
using Netension.Event.Sample.Enumerations;
using Netension.Extensions.Correlation;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
                    builder.UseRabbitMQ((options, configuration) => { configuration.GetSection("RabbitMQ").Bind(options); options.Password = "guest".Encrypt(); });
=======

                    builder.UseRabbitMQ(EventingEnumerations.RabbitMQ);
>>>>>>> Stashed changes

                    builder.RegistrateEventHandlers(register => register.RegistrateHandlerFromAssemblyOf<Startup>());

                    builder.RegistrateEventPublishers((register) =>
                    {
<<<<<<< Updated upstream
                        register.RegistrateRabbitMQPublisher("rabbitmq", "RabbitMQ", (@event) => true, (options, configuration) => configuration.GetSection("RabbitMQ:Publish").Bind(options));
=======
                        register.RegistrateRabbitMQPublisher(EventingEnumerations.Publishers.Publisher);
>>>>>>> Stashed changes
                    });

                    builder.RegistrateEventListeners((register) =>
                    {
<<<<<<< Updated upstream
                        register.RegistrateRabbitMQListener("rabbitmq", "RabbitMQ", (options, configuration) => configuration.GetSection("RabbitMQ:Listen").Bind(options));
=======
                        register.RegistrateRabbitMQListener(EventingEnumerations.Listeners.Listener);
>>>>>>> Stashed changes
                    });

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
