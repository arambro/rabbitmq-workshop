using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using game.Infrastructure;
using game.Messages;

namespace game.Players
{
    public class Player : Component
    {
        private const int RandomRange = 100;
        private readonly Dictionary<Position, int> finishingChance = new Dictionary<Position, int>
        {
            {Position.GoalKeeper, 1},
            {Position.Defender, 2},
            {Position.Winger, 5},
            {Position.Midfielder, 5},
            {Position.Striker, 7}
        };

        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Random random;

        public Player(string id, int number, string name, Position position)
            : base(
                id,
                RabbitMQContext.PlayersExchange,

                // TODO: SET THE CORRECT ROUTING KEY
                string.Empty)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.random = new Random(Guid.NewGuid().GetHashCode());

            this.Number = number;
            this.Name = name;
            this.Position = position;
        }

        public int Number { get; set; }

        public string Name { get; set; }

        public Position Position { get; set; }

        public Team Team { get; set; }

        public int ScoredGoals { get; set; }

        protected override void ConsumeMatchStarted(MatchStarted message)
        {
            var token = this.cancellationTokenSource.Token;
            Console.ForegroundColor = this.Team.ConsoleColor;
            Console.WriteLine($"{this.Number} - {this.Name} joins the game");
            Task.Factory.StartNew(
                    () => { Play(token); },
                    token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default
                )
                .ConfigureAwait(false);
        }

        protected override void ConsumeMatchFinished(MatchFinished message)
        {
            // TODO: WHAT CAN WE DO HERE?
            base.ConsumeMatchFinished(message);
        }

        private void Play(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (this.TryScore())
                {
                    this.PublishToReferees(
                        new Goal
                        {
                            Team = this.Team.Name,
                            PlayerNumber = this.Number,
                            PlayerName = this.Name
                        },

                        // TODO: SET THE CORRECT ROUTING_KEY
                        $"{this.Number}.goal");
                }
                Thread.Sleep(1000);
            }
        }

        private bool TryScore()
        {
            var goalChance = this.finishingChance[this.Position];
            if (this.random.Next(1, RandomRange) <= goalChance)
            {
                this.ScoredGoals++;
                Console.ForegroundColor = this.Team.ConsoleColor;
                Console.WriteLine($"{Environment.NewLine}{this.Number} - {this.Name} scores!, personal count: {this.ScoredGoals} goal(s)");
                return true;
            }

            return false;
        }
    }
}