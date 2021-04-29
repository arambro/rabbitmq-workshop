using System;
using System.Collections.Concurrent;
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
        private readonly ConsoleColor consoleColor;
        private Team[] teams;

        public Referee()
            : base("referee")
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.scores = new ConcurrentDictionary<string, int>();
            this.consoleColor = ConsoleColor.DarkGreen;

            // TODO: 2. CREATE ALL THE NECESSARY QUEUES AND BIND THEM USING THE APPROPRIATE ROUTING KEY. (WHERE THE REFEREE WILL SUBSCRIBE)
            this.CreateAndBindQueue(
                this.Id, // QUEUE NAME
                RabbitMQContext.RefereesExchange, // EXCHANGE NAME
                "*.validgoal"); // ROUTING KEY
        }

        public void StartMatch(params Team[] teams)
        {
            this.teams = teams;
            foreach (var team in teams)
            {
                this.scores[team.Name] = 0;
            }

            this.StartTimer();

            // TODO: 5. PUBLISH THIS MESSAGE ALL THE NECESSARY TIMES WITH THE CORRECT EXCHANGE AND ROUTING KEY FOR THE PLAYERS
            this.PublishToPlayers(new MatchStarted());

            Console.ForegroundColor = this.consoleColor;
            Console.WriteLine($"{Environment.NewLine}MATCH STARTED");

            var timespan = (int)(this.matchTime * 1.5);
            this.cancellationTokenSource.Token.WaitHandle.WaitOne(timespan);
            Console.WriteLine($"{Environment.NewLine}...PRESS A KEY TO EXIT...");
            Console.ReadKey();
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
            Console.ForegroundColor = this.consoleColor;
            Console.WriteLine($"{Environment.NewLine}MATCH ENDED");
            Console.WriteLine("The match final score is:");
            foreach (var (team, score) in this.scores)
            {
                Console.WriteLine($"{team}: {score}");
            }

            // TODO: 8. PUBLISH THIS MESSAGE ALL THE NECESSARY TIMES WITH THE CORRECT EXCHANGE AND ROUTING KEY FOR THE PLAYERS
            this.PublishToPlayers(new MatchFinished());

            Task.Delay(1000, this.cancellationTokenSource.Token);
            this.cancellationTokenSource.Cancel(false);
        }
    }
}
