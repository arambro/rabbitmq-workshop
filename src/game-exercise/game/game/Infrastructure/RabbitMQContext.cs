using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace game.Infrastructure
{
    public class RabbitMQContext
    {
        public const string PlayersExchange = "players_exchange";
        public const string RefereesExchange = "referees_exchange";

        private IConnection connection;
        private IModel model;

        public RabbitMQContext()
        {
            var connectionFactory = new ConnectionFactory
            {
                UserName = "admin",
                Password = "1234",
                HostName = "localhost",
                Port = 5672
            };

            this.connection = connectionFactory.CreateConnection();
            this.model = this.connection.CreateModel();
        }

        public event EventHandler<BasicDeliverEventArgs> Received;

        internal void SetupExchanges()
        {
            this.model.ExchangeDeclare(
                PlayersExchange,

                // TODO: SET THE CORRECT EXCHANGE TYPE
                ExchangeType.Fanout,
                durable: true);

            this.model.ExchangeDeclare(
                RefereesExchange,

                // TODO: SET THE CORRECT EXCHANGE TYPE
                ExchangeType.Topic,
                durable: true);
        }

        internal void SetUpQueue(string queueName, string exchangeName, string routingKey)
        {
            this.model.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
            this.model.QueueBind(queueName, exchangeName, routingKey);
        }

        internal void ConsumeQueue(string queueName)
        {
            var consumer = new EventingBasicConsumer(this.model);
            consumer.Received += this.Received;
            this.model.BasicConsume(queueName, autoAck: true, consumer: consumer);
        }

        internal object ExtractMessage(BasicDeliverEventArgs args, Type type)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(message, type);
        }

        internal void PublishMessage<T>(string exchangeName, string routingKey, T message)
        {
            var basicProperties = this.model.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>
            {
                { "Content-Type", "application/json" }
            };
            basicProperties.Type = typeof(T).FullName;

            var jsonSerializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            var messageBytes = Encoding.UTF8.GetBytes(jsonSerializedMessage);

            this.model.BasicPublish(exchangeName, routingKey, basicProperties, messageBytes);
        }
    }
}
