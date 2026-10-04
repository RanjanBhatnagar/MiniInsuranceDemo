using System.Text;
using System.Text.Json;
using InsuranceAPI.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace InsuranceAPI.Messaging;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;

    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine(
                    "Attempting to connect to RabbitMQ...");

                await ConnectAndConsumeAsync(stoppingToken);

                // If connection exits normally, wait before retrying.
                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "RabbitMQ connection failed.");
                Console.WriteLine(
                    $"Error: {ex.Message}");
                Console.WriteLine(
                    "Retrying in 5 seconds...");
                Console.WriteLine();

                await Task.Delay(
                    TimeSpan.FromSeconds(5),
                    stoppingToken);
            }
        }
    }

    private async Task ConnectAndConsumeAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName =
                _configuration["RabbitMQ:Host"]
                ?? "localhost",

            UserName =
                _configuration["RabbitMQ:Username"]
                ?? "guest",

            Password =
                _configuration["RabbitMQ:Password"]
                ?? "guest"
        };

        _connection =
            await factory.CreateConnectionAsync();

        _channel =
            await _connection.CreateChannelAsync();

        Console.WriteLine(
            "Successfully connected to RabbitMQ.");

        await _channel.QueueDeclareAsync(
            queue: "customer-created-queue",
            durable: true,
            exclusive: false,
            autoDelete: false);

        var consumer =
            new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var json =
                    Encoding.UTF8.GetString(
                        args.Body.ToArray());

                var customerEvent =
                    JsonSerializer.Deserialize<CustomerCreatedEvent>(
                        json);

                if (customerEvent != null)
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "========================================");
                    Console.WriteLine(
                        "RABBITMQ MESSAGE RECEIVED");
                    Console.WriteLine(
                        "========================================");

                    Console.WriteLine(
                        $"Customer Id : {customerEvent.CustomerId}");

                    Console.WriteLine(
                        $"Name        : {customerEvent.Name}");

                    Console.WriteLine(
                        $"Email       : {customerEvent.Email}");

                    Console.WriteLine(
                        "========================================");
                    Console.WriteLine();
                }

                await _channel.BasicAckAsync(
                    args.DeliveryTag,
                    multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error processing RabbitMQ message: {ex.Message}");
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "customer-created-queue",
            autoAck: false,
            consumer: consumer);

        // Keep this consumer alive.
        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            if (_channel != null)
                await _channel.CloseAsync();

            if (_connection != null)
                await _connection.CloseAsync();
        }
        catch
        {
            // Ignore shutdown errors.
        }

        await base.StopAsync(cancellationToken);
    }
}