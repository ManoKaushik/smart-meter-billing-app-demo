using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SmartMeterWeb.Interfaces;
using SmartMeterWeb.Models.Billing;


namespace SmartMeterWeb.Services
{
    public class RabbitmqReadingService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private ILogger _logger;
        private const string ExchangeName = "your_exchange_name";
        private const string QueueName = "your_queue_name";
        private const string RoutingKey = "your_routing_key";

        public RabbitmqReadingService(IServiceScopeFactory scopeFactory, ILogger<RabbitmqReadingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _factory = new ConnectionFactory() { HostName = "localhost", VirtualHost = "your_vhost", UserName = "your_username", Password = "your_password" };
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                try
                {
                    var dto = JsonSerializer.Deserialize<MeterReadingDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (dto != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var readingService = scope.ServiceProvider.GetRequiredService<IMeterReadingService>();

                        await readingService.RecordReadingAsync(dto);
                        _logger.LogInformation("Saved reading for meter {MeterId} at {Time}", dto.MeterId, dto.ReadingDate);
                    }

                    await _channel.BasicAckAsync(args.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    await _channel.BasicNackAsync(args.DeliveryTag, false, true); // requeue if failure
                }
            };

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            base.Dispose();
        }
    }
}

