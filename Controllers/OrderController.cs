using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApp.Models;
using RabbitMQ.Client;
using System.Text;

namespace OrderApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrder(Order order)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri("amqps://lydstqct:vNuRA5v2vdCYsGmd2WmCq84zLWSFD_xt@jaragua.lmq.cloudamqp.com/lydstqct"),
                    Ssl = { Enabled = true } // Importante: SSL ativado
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: "OrderQueue", durable: false, exclusive: false, autoDelete: false,
                    arguments: null);

                const string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "OrderQueue", body: body);
                Console.WriteLine($" [x] Sent {message}");
                return Accepted(order);
            }
            catch (Exception)
            {
                _logger.LogError("Erro");
                return new StatusCodeResult(500);
            }
        }
    }
}

