using System;
using System.Collections.Generic;
using game.Messages;
using RabbitMQ.Client.Events;

namespace game.Infrastructure
{
    public class Component
    {
        private readonly RabbitMQContext rabbitMqContext;

        protected string Id;

        public Component(string id, string exchange, string routingKey)
        {
            this.rabbitMqContext = new RabbitMQContext();

            this.Id = id;
            var queueName = $"{this.Id}_queue";
            
            this.rabbitMqContext.SetUpQueue(queueName, exchange, routingKey);
            this.rabbitMqContext.Received += Consume;
            this.rabbitMqContext.ConsumeQueue(queueName);
        }

        protected void Publish<T>(T message, string exchangeName, string routingKey)
        {
            this.rabbitMqContext.PublishMessage(exchangeName, routingKey, message);
        }

        protected virtual void ConsumeMatchStarted(MatchStarted message)
        {
        }

        protected virtual void ConsumeGoal(Goal message)
        {
        }

        protected virtual void ConsumeValidGoal(ValidGoal validGoal)
        {
        }

        protected virtual void ConsumeMatchFinished(MatchFinished matchFinished)
        {
        }

        private void Consume(object sender, BasicDeliverEventArgs args)
        {
            var allowedMessages = new Dictionary<string, Type>
            {
                {typeof(MatchStarted).FullName, typeof(MatchStarted)},
                {typeof(ValidGoal).FullName, typeof(ValidGoal)},
                {typeof(Goal).FullName, typeof(Goal)},
                {typeof(MatchFinished).FullName, typeof(MatchFinished)},
            };
            var messageType = allowedMessages[args.BasicProperties.Type];
            var deserializedMessage = this.rabbitMqContext.ExtractMessage(args, messageType);
            switch (deserializedMessage)
            {
                case MatchStarted matchStarted:
                    this.ConsumeMatchStarted(matchStarted);
                    break;

                case ValidGoal validGoal:
                    this.ConsumeValidGoal(validGoal);
                    break;

                case Goal goal:
                    this.ConsumeGoal(goal);
                    break;

                case MatchFinished matchFinished:
                    this.ConsumeMatchFinished(matchFinished);
                    break;
            }
        }
    }
}
