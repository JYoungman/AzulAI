﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzulAI
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Player> AIs = new List<Player>();
            // AIs.Add(new PureRandom());
            // AIs.Add(new BGCplayer());
            // AIs.Add(new QuickEndPlayer());
            AIs.Add(new SimpleGreedyPlayer());
            AIs.Add(new CentrestGreedyPlayer());
            AIs.Add(new SimpleGreedyPlayer());
            AIs.Add(new SimpleGreedyPlayer());

            var rounds = 10000;
            Console.WriteLine($"Beginning {rounds} round match with {AIs.Count} players.");
            for(int i = 0; i < AIs.Count; i++)
            {
                Console.WriteLine("Player "+(i+1)+": " + AIs[i].DisplayName() + "");
            }

            var tournament = new Tournament(AIs, rounds);
            var results = tournament.PlayTournament();

            Console.WriteLine();
            Console.WriteLine("Results:");
            for(int i = 0; i < AIs.Count; i++)
            {
                Console.WriteLine($"Player {i + 1}: {results.WinPercentages[i]:P1} Avg Score: {results.AverageScores[i]:F1}, Avg Earned Penalty: {results.AverageEarnedPenalties[i]:F1}, Avg Applied Penalty: {results.AverageAppliedPenalties[i]:F1}  {AIs[i].DisplayName()} ");
            }

            Console.WriteLine();
            Console.WriteLine($"Avg Game Length: {results.AverageRounds:F}");
            Console.WriteLine($"Ties: {results.Ties}"); ;
            Console.WriteLine($"Time: {results.Time.TotalSeconds:F}s");


            Console.WriteLine("Press Enter to exit.");
            Console.Read();
             
        }
    }
}
