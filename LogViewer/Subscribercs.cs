using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LogViewer;

public class Subscriber
{
    private readonly string _hostName =
    Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
    private readonly int _port =
        int.TryParse(Environment.GetEnvironmentVariable("RABBITMQ_PORT"), out var p) ? p : 5672;
    private readonly string _userName =
        Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
    private readonly string _password =
        Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";

    private readonly string _exchangeName = "logs-topic";
    private readonly string _topic;
    public Subscriber(string topic)
    {
        _topic = topic;
    }

    public void Start()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostName,
            Port = _port,
            UserName = _userName,
            Password = _password
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Topic, //fanout
            durable: true);

        var tempQueue = channel.QueueDeclare(
            queue: "",
            durable: false,
            exclusive: true,
            autoDelete: true,
            arguments: null);

        var queueName = tempQueue.QueueName;
        //Suscripcion al topic indicado
        channel.QueueBind(
            queue: queueName,
            exchange: _exchangeName,
            routingKey: _topic);

        Console.WriteLine($"LogViewer suscrito a topic '{_topic}'.\n");

        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var logEntry = JsonSerializer.Deserialize<LogEntry>(json);

            if (logEntry != null)
            {
                Console.WriteLine(
                    $"[{logEntry.Timestamp:u}] {logEntry.LogLevel} {logEntry.Category}: {logEntry.Message}");

                if (!string.IsNullOrEmpty(logEntry.Exception))
                {
                    Console.WriteLine($"  Exception: {logEntry.Exception}");
                }
            }
        };

        channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer: consumer);

        Console.CancelKeyPress += (_, e) => { e.Cancel = true; };
        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    private class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string? LogLevel { get; set; }
        public string? Category { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? Message { get; set; }
        public string? Exception { get; set; }
    }
}
