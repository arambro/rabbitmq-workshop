using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using game.Infrastructure;
using game.Messages;

namespace game.Players
{
    public class Referee : Component
    {
        private int matchTime = 30000;
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly ConcurrentDictionary<string, int> scores;

        public Referee()
            : base(
                "referee",
                RabbitMQContext.RefereesExchange,

                // TODO: SET THE CORRECT ROUTING KEY
                "*.validgoal")
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.scores = new ConcurrentDictionary<string, int>();
        }

        public void StartMatch(params Team[] teams)
        {
            foreach (var team in teams)
            {
                this.scores[team.Name] = 0;
            }

            this.StartTimer();

            this.PublishToPlayers(
                new MatchStarted(),

                // TODO: SET THE CORRECT ROUTING_KEY
                "");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("MATCH STARTED");

            var timespan = (int) (this.matchTime * 1.5);
            this.cancellationTokenSource.Token.WaitHandle.WaitOne(timespan);
        }

        protected override void ConsumeValidGoal(ValidGoal validGoal)
        {
            this.scores.AddOrUpdate(validGoal.Team, 1, (team, score) => ++score);
        }

        private void StartTimer()
        {
            var token = this.cancellationTokenSource.Token;
            Task.Delay(this.matchTime, token)
                .ContinueWith(
                    act => { this.EndMatch(); },
                    token,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);
        }

        private void EndMatch()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{Environment.NewLine}MATCH ENDED");
            Console.WriteLine("The match final score is:");
            foreach (var (team, score) in this.scores)
            {
                Console.WriteLine($"{team}: {score}");
            }

            this.PublishToPlayers(
                new MatchFinished(),

                // TODO: SET THE CORRECT ROUTING_KEY
                "");

            Task.Delay(1000, this.cancellationTokenSource.Token);

            this.cancellationTokenSource.Cancel(false);
        }
    }
}
