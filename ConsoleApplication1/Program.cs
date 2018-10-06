using System;
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
            AIs.Add(new Player());
            AIs.Add(new SimpleGreedyPlayer());
            AIs.Add(new SimpleGreedyPlayer());
            AIs.Add(new QuickEndPlayer());

            Console.WriteLine("Beginning match with " + AIs.Count + " players.");
            for(int i = 0; i < AIs.Count; i++)
            {
                Console.WriteLine("Player "+(i+1)+": " + AIs[i].DisplayName() + "");
            }

            var tournament = new Tournament(AIs, 1000);
            var results = tournament.PlayTournament();

            Console.WriteLine();
            Console.WriteLine("Results:");
            for(int i = 0; i < AIs.Count; i++)
            {
                Console.WriteLine($"Player {i + 1}: {results.WinPercentages[i]:P1} Avg Score: {results.AverageScores[i]:F1} {AIs[i].DisplayName()} ");
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
