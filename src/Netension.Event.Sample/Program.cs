using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
                    builder.RegistrateEventHandlers(register => register.RegistrateHandlerFromAssemblyOf<Startup>());

                    builder.UseRabbitMQ((options, configuration) => { configuration.GetSection("RabbitMQ").Bind(options); options.Password = "guest".Encrypt(); }, (builder) =>
                    {
                        builder.AddListener("Listener", (options, configuration) => configuration.GetSection("RabbitMQ:Listen").Bind(options));

                        builder.AddPublisher("Publisher", (options, configuration) => configuration.GetSection("RabbitMQ:Publish").Bind(options), (@event) => true);
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
