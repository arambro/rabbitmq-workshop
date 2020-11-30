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
            : base(
                "VAR",
                RabbitMQContext.RefereesExchange,

                // TODO: SET THE CORRECT ROUTING KEY
                "*.goal")
        {
            this.random = new Random(Guid.NewGuid().GetHashCode());
        }

        protected override void ConsumeGoal(Goal message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{Environment.NewLine}{this.Id} needs to validate {message.PlayerName}'s goal...");
            Thread.Sleep(100);

            if (this.ValidateGoal(message))
            {
                this.PublishToReferees(new ValidGoal(message), $"{message.Team}.validgoal");
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
