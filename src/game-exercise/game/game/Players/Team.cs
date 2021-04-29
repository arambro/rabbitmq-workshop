using System;
using System.Collections.Generic;

namespace game.Players
{
    public class Team
    {
        public Team(string name, List<Player> players, ConsoleColor consoleColor)
        {
            players.ForEach(p => p.Team = this);

            this.Name = name;
            this.Players = players;
            this.ConsoleColor = consoleColor;
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"{Environment.NewLine}{name}'s players are ready to join...{Environment.NewLine}");
        }

        public string Name { get; set; }

        public List<Player> Players { get; set; }

        public ConsoleColor ConsoleColor { get; set; }
    }
}