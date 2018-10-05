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
            AIs.Add(new QuickEndPlayer());
            AIs.Add(new SimpleGreedyPlayer());
            AIs.Add(new SimpleGreedyPlayer());
            AIs.Add(new QuickEndPlayer());

            Console.Write("Beginning match with " + AIs.Count + " players.\n");
            for(int i = 0; i < AIs.Count; i++)
            {
                Console.Write("Player "+(i+1)+": " + AIs[i].DisplayName() + "\n");
            }
            
            GameManager gm = new GameManager(AIs);
            GameResults results = gm.PlayGame();

            Console.Write("\nResults:\n");
            for(int i = 0; i < AIs.Count; i++)
            {
                Console.Write("Player " + (i + 1) + ": " + AIs[i].DisplayName() + " " + results.players[i].score + "\n");
            }
            
            Console.Write("Winner(s): ");
            for (int i = 0; i < results.winners.Count; i++ )
            {
                Console.Write(results.winners[i].DisplayName() + " ");
            }
            Console.Write("\n");

            Console.Write("Game Length: " + results.roundCount + "\n");

            Console.Write("Press Enter to exit.");
            Console.Read();
             
        }
    }
}
