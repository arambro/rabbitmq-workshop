using System;
using System.Collections.Generic;
using game.Infrastructure;
using game.Players;

namespace game
{
    class Program
    {
        public static void Main(string[] args) 
        {
            var matchContext = new RabbitMQContext();
            matchContext.SetupExchanges();

            var referee = new Referee();
            new VAR();

            Console.ForegroundColor = ConsoleColor.White;
            var madrid = new Team(
                "REAL MADRID",
                new List<Player>
                {
                    new Player("casillas", 1, "Iker Casillas", Position.GoalKeeper),

                    new Player("varane", 2, "Varane", Position.Defender),
                    new Player("pepe", 3, "Pepe", Position.Defender),
                    new Player("carvajal", 15, "Carvajal", Position.Defender),
                    new Player("marcelo", 12, "Marcelo", Position.Defender),

                    new Player("ramos", 4, "Sergio Ramos", Position.Midfielder),
                    new Player("kroos", 8, "Toni Kroos", Position.Midfielder),
                    new Player("james", 10, "James Rodriguez", Position.Winger),
                    new Player("isco", 23, "Isco", Position.Winger),

                    new Player("bale", 11, "Gareth Bale", Position.Striker),
                    new Player("cristiano", 7, "Cristiano Ronaldo", Position.Striker),
                },
                ConsoleColor.White);

            Console.ForegroundColor = ConsoleColor.Blue;
            var barcelona = new Team(
                "BARÇA",
                new List<Player>
                {
                    new Player("terStegen", 1, "Marc Andre Ter Stegen", Position.GoalKeeper),

                    new Player("alves", 22, "Dani Alves", Position.Defender),
                    new Player("pique", 3, "Gerard Pique", Position.Defender),
                    new Player("mascherano", 14, "Javier Mascherano", Position.Defender),
                    new Player("alba", 18, "Jordi Alba", Position.Defender),

                    new Player("busquets", 5, "Sergio Busquets", Position.Midfielder),
                    new Player("xavi", 6, "Xavi Hernandez", Position.Midfielder),
                    new Player("iniesta", 8, "Andres Iniesta", Position.Midfielder),

                    new Player("suarez", 9, "Luis Suarez", Position.Striker),
                    new Player("messi", 10, "Lionel Messi", Position.Striker),
                    new Player("neymar", 11, "Neymar Jr", Position.Winger),
                },
                ConsoleColor.Blue);

            referee.StartMatch(madrid, barcelona);
        }
    }
}
