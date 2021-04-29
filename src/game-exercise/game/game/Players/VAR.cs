using System;
using System.Threading;
using game.Infrastructure;
using game.Messages;

namespace game.Players
{
    public class VAR : Component
    {
        private readonly Random random;

        public VAR()
            : base("VAR")
        {
            this.random = new Random(Guid.NewGuid().GetHashCode());

            // TODO: 4. CREATE ALL THE NECESSARY QUEUES AND BIND THEM USING THE APPROPRIATE ROUTING KEY. (WHERE THE VAR WILL SUBSCRIBE)
            this.CreateAndBindQueue(
                this.Id, // QUEUE NAME
                RabbitMQContext.RefereesExchange, // EXCHANGE NAME
                "*.goal"); // ROUTING KEY
        }

        protected override void ConsumeGoal(Goal message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Environment.NewLine}{this.Id} needs to validate {message.PlayerName}'s goal...");
            Thread.Sleep(100);

            if (this.ValidateGoal(message))
            {
                // TODO: 7. PUBLISH THIS MESSAGE WITH THE CORRECT EXCHANGE AND ROUTING KEY TO THE REFEREE
                this.PublishToReferees(
                    new ValidGoal(message),
                    $"{message.Team}.validgoal");
            }
        }

        public bool ValidateGoal(Goal message)
        {
            var goalChance = 30;
            if (this.random.Next(1, 100) > goalChance)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}{this.Id} declares INVALID {message.PlayerName}'s goal");
                return false;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Environment.NewLine}GOAAAAAAAAAL FOR {message.Team} - {message.PlayerNumber} - {message.PlayerName.ToUpperInvariant()}!!!!!!!!!!!!!!");
            return true;
        }
    }
}