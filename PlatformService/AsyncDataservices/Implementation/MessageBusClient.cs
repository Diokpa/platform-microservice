using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using PlatformService.AsyncDataservices.Interface;
using PlatformService.DTOs;
using RabbitMQ.Client;
using System.Runtime.CompilerServices;
using System.Text;

namespace PlatformService.AsyncDataservices.Implementation
{
  public class MessageBusClient : IMessageBusClient
  {
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
      _configuration = configuration;
      // setup rabbitmq connection factory with values from appsetting
      var factory = new ConnectionFactory()
      {
        HostName = _configuration["RabbitMQHost"],
        Port = int.Parse(_configuration["RabbitMQPort"])
      };
      try
      {
        // Steps for rabbitmq setup
        // 1. Set up a Connection
        _connection = factory.CreateConnection();
        // 2. Setup a Channel
        _channel = _connection.CreateModel();
        // 3. Set up the Exchange type from the Channel
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        Console.WriteLine("--> Connected to MessageBus");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
      }
    }
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
      var message = JsonSerializer.Serialize(platformPublishedDto);
      if (_connection.IsOpen)
      {
        Console.WriteLine("--> RabbitMQ Connection is Open, sending message...");
        // Sending the message
        SendMessage(message);
      }
      else
      {
        Console.WriteLine("--> RabbitMQ Connection is Closed, not sending message...");
      }
    }

    private void SendMessage(string message)
    {
      var body = Encoding.UTF8.GetBytes(message);

      // Publish message with th name "trigger" of the specified Exchange
      // Ignore Routing Key for Fanout Exchange
      _channel.BasicPublish(exchange: "trigger",
                        routingKey: "",
                        basicProperties: null,
                        body: body);
      Console.WriteLine($"--> We have sent {message}");
    }

    public void Dispose()
    {
      Console.WriteLine("MessageBus Disposed");
      if (_channel.IsOpen)
      {
        _channel.Close();
        _connection.Close();
      }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
      Console.WriteLine("-->  RabbitMQ Connection Shutdown");
    }
  }
}