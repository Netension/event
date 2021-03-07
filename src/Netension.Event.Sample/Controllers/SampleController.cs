using Microsoft.AspNetCore.Mvc;
using Netension.Event.Abstraction;
using Netension.Event.Sample.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Netension.Event.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IEventPublisher _publisher;

        public SampleController(IEventPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpGet]
        public async Task Get()
        {
            await _publisher.PublishAsync(new SampleEvent("Hello World!"), "routingkey", CancellationToken.None);
        }
    }
}
